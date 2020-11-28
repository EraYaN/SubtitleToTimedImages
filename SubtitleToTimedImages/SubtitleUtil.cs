using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.BluRaySup;
using Nikse.SubtitleEdit.Core.ContainerFormats.Matroska;
using Nikse.SubtitleEdit.Core.VobSub;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

namespace SubtitleToTimedImages
{
    internal static class SubtitleUtil
    {
        public static List<SubtitleEvent> ReadSup(string inputFile)
        {
            List<SubtitleEvent> events = new List<SubtitleEvent>();
            StringBuilder logger = new StringBuilder();
            var result = BluRaySupParser.ParseBluRaySup(inputFile, logger);
            int i = 0;
            foreach (var line in result)
            {
                var bitmap = BluRaySupParser.SupDecoder.DecodeImage(line.PcsObjects[0], line.BitmapObjects[0], line.PaletteInfos);

                var image_data = ImageUtil.ConvertToPngBytes(bitmap);
                events.Add(new SubtitleEvent() { StartTime = TimeSpan.FromMilliseconds(line.StartTime / 90.0), EndTime = TimeSpan.FromMilliseconds(line.EndTime / 90.0), MimeType = "image/png", Image = image_data, Origin = line.PcsObjects[0].Origin });
                i++;
            }
            return events;
        }

        public static List<SubtitleEvent> ReadMatroskaBluraySup(string inputFile, int tracknumber)
        {
            List<SubtitleEvent> events = new List<SubtitleEvent>();
            var matroska = new MatroskaFile(inputFile);
            var matroskaSubtitleInfo = matroska.GetTracks()[tracknumber];

            var sub = matroska.GetSubtitle(matroskaSubtitleInfo.TrackNumber, (p, t) => Console.WriteLine("Progress: {0} out of {1}", p, t));

            var subtitles = new List<BluRaySupParser.PcsData>();
            var log = new StringBuilder();
            var clusterStream = new MemoryStream();
            var lastPalettes = new Dictionary<int, List<PaletteInfo>>();
            var lastBitmapObjects = new Dictionary<int, List<BluRaySupParser.OdsData>>();
            foreach (var p in sub)
            {
                byte[] buffer = p.GetData(matroskaSubtitleInfo);
                if (buffer != null && buffer.Length > 2)
                {
                    clusterStream.Write(buffer, 0, buffer.Length);
                    if (ContainsBluRayStartSegment(buffer))
                    {
                        if (subtitles.Count > 0 && subtitles[subtitles.Count - 1].StartTime == subtitles[subtitles.Count - 1].EndTime)
                        {
                            subtitles[subtitles.Count - 1].EndTime = (long)((p.Start - 1) * 90.0);
                        }
                        clusterStream.Position = 0;
                        var list = BluRaySupParser.ParseBluRaySup(clusterStream, log, true, lastPalettes, lastBitmapObjects);
                        foreach (var sup in list)
                        {
                            sup.StartTime = (long)((p.Start - 1) * 90.0);
                            sup.EndTime = (long)((p.End - 1) * 90.0);
                            subtitles.Add(sup);

                            // fix overlapping
                            if (subtitles.Count > 1 && sub[subtitles.Count - 2].End > sub[subtitles.Count - 1].Start)
                            {
                                subtitles[subtitles.Count - 2].EndTime = subtitles[subtitles.Count - 1].StartTime - 1;
                            }
                        }
                        clusterStream = new MemoryStream();
                    }
                }
                else if (subtitles.Count > 0)
                {
                    var lastSub = subtitles[subtitles.Count - 1];
                    if (lastSub.StartTime == lastSub.EndTime)
                    {
                        lastSub.EndTime = (long)((p.Start - 1) * 90.0);
                        if (lastSub.EndTime - lastSub.StartTime > 1000000)
                        {
                            lastSub.EndTime = lastSub.StartTime;
                        }
                    }
                }
            }

            clusterStream.Dispose();

            int i = 0;
            foreach (var line in subtitles)
            {
                var bitmap = BluRaySupParser.SupDecoder.DecodeImage(line.PcsObjects[0], line.BitmapObjects[0], line.PaletteInfos);

                var image_data = ImageUtil.ConvertToPngBytes(bitmap);
                events.Add(new SubtitleEvent() { StartTime = TimeSpan.FromMilliseconds(line.StartTime / 90.0), EndTime = TimeSpan.FromMilliseconds(line.EndTime / 90.0), MimeType = "image/png", Image = image_data, Origin = line.PcsObjects[0].Origin });
                i++;
            }
            return events;
        }

        private static bool ContainsBluRayStartSegment(byte[] buffer)
        {
            const int epochStart = 0x80;
            var position = 0;
            while (position + 3 <= buffer.Length)
            {
                var segmentType = buffer[position];
                if (segmentType == epochStart)
                {
                    return true;
                }

                int length = BluRaySupParser.BigEndianInt16(buffer, position + 1) + 3;
                position += length;
            }
            return false;
        }

        public static List<SubtitleEvent> ReadMatroskaVobSub(string inputFile, int tracknumber)
        {
            var matroska = new MatroskaFile(inputFile);
            var matroskaSubtitleInfo = matroska.GetTracks()[tracknumber];

            var subtitle = matroska.GetSubtitle(matroskaSubtitleInfo.TrackNumber, (p, t) => Console.WriteLine("Progress: {0} out of {1}",p,t));
            

            var mergedVobSubPacks = new List<VobSubMergedPack>();
            if (matroskaSubtitleInfo.ContentEncodingType == 1)
            {
                throw new ArgumentException("Track number is invalid.");
            }

            var sub = matroska.GetSubtitle(matroskaSubtitleInfo.TrackNumber, null);
            var idx = new Idx(matroskaSubtitleInfo.GetCodecPrivate().SplitToLines());
            foreach (var p in sub)
            {
                mergedVobSubPacks.Add(new VobSubMergedPack(p.GetData(matroskaSubtitleInfo), TimeSpan.FromMilliseconds(p.Start), 32, null));
                if (mergedVobSubPacks.Count > 0)
                {
                    mergedVobSubPacks[mergedVobSubPacks.Count - 1].EndTime = TimeSpan.FromMilliseconds(p.End);
                }

                // fix overlapping (some versions of Handbrake makes overlapping time codes - thx Hawke)
                if (mergedVobSubPacks.Count > 1 && mergedVobSubPacks[mergedVobSubPacks.Count - 2].EndTime > mergedVobSubPacks[mergedVobSubPacks.Count - 1].StartTime)
                {
                    mergedVobSubPacks[mergedVobSubPacks.Count - 2].EndTime = TimeSpan.FromMilliseconds(mergedVobSubPacks[mergedVobSubPacks.Count - 1].StartTime.TotalMilliseconds - 1);
                }
            }


            List<SubtitleEvent> events = new List<SubtitleEvent>();
            StringBuilder logger = new StringBuilder();
            var MergedVobSubPacks = mergedVobSubPacks;
            int i = 0;
            foreach (var line in MergedVobSubPacks)
            {
                var bitmap = line.SubPicture.GetBitmap(idx.Palette, Color.Black, Color.White, Color.Gray, Color.White, false);

                var image_data = ImageUtil.ConvertToPngBytes(bitmap);
                events.Add(new SubtitleEvent() { StartTime = line.StartTime, EndTime = line.EndTime, MimeType = "image/png", Image = image_data, Origin = line.SubPicture.ImageDisplayArea.Location });
                i++;
            }
            return events;
        }

        public static List<SubtitleEvent> ReadVobSub(string inputFile, string idxFile)
        {
            List<SubtitleEvent> events = new List<SubtitleEvent>();
            StringBuilder logger = new StringBuilder();
            var vobsubparser = new VobSubParser(false);
            vobsubparser.OpenSubIdx(inputFile, idxFile);
            var MergedVobSubPacks = vobsubparser.MergeVobSubPacks();
            int i = 0;
            foreach (var line in MergedVobSubPacks)
            {
                var bitmap = line.SubPicture.GetBitmap(vobsubparser.IdxPalette, Color.Black, Color.White, Color.Gray, Color.White, false);

                var image_data = ImageUtil.ConvertToPngBytes(bitmap);
                events.Add(new SubtitleEvent() { StartTime = line.StartTime, EndTime = line.EndTime, MimeType = "image/png", Image = image_data, Origin = line.SubPicture.ImageDisplayArea.Location });
                i++;
            }
            return events;
        }
    }
}
