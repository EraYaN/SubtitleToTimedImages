using Nikse.SubtitleEdit.Core.BluRaySup;
using Nikse.SubtitleEdit.Core.VobSub;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
