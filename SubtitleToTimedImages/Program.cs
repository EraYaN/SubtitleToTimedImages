using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace SubtitleToTimedImages
{
    class Program
    {
        const string MainPath = @"E:\subtitle_parsing_examples";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var events = SubtitleUtil.ReadMatroskaVobSub(Path.Combine(MainPath, "vobsub_example.mkv"), 0); //special subtitle only matroska files so track 0
            sw.Stop();
            Console.WriteLine("Parsed Matroska VobSub in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            sw.Restart();
            using (StreamWriter file = File.CreateText(Path.Combine(MainPath, "vobsub_example_out.json")))
            {
                serializer.Serialize(file, events);
            }
            sw.Stop();
            Console.WriteLine("Saved Matroska VobSub JSON in {0} ms", sw.ElapsedMilliseconds);

            sw.Restart();
            events = SubtitleUtil.ReadMatroskaBluraySup(Path.Combine(MainPath, "bluraysup_example.mkv"), 0); //special subtitle only matroska files so track 0
            sw.Stop();
            Console.WriteLine("Parsed Matroska Sup in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            sw.Restart();
            using (StreamWriter file = File.CreateText(Path.Combine(MainPath, "bluraysup_example_out.json")))
            {
                serializer.Serialize(file, events);
            }
            sw.Stop();
            Console.WriteLine("Saved Matroska Sup JSON in {0} ms", sw.ElapsedMilliseconds);

            sw.Restart();
            events = SubtitleUtil.ReadVobSub(Path.Combine(MainPath, "vobsub_example.sub"), Path.Combine(MainPath, "vobsub_example.idx"));
            sw.Stop();
            Console.WriteLine("Parsed VobSub in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            sw.Restart();
            using (StreamWriter file = File.CreateText(Path.Combine(MainPath, "vobsub_example_out2.json")))
            {
                serializer.Serialize(file, events);
            }
            sw.Stop();
            Console.WriteLine("Saved VobSub JSON in {0} ms", sw.ElapsedMilliseconds);

            sw.Restart();
            events = SubtitleUtil.ReadSup(Path.Combine(MainPath, "bluraysup_example.sup"));
            sw.Stop();
            Console.WriteLine("Parsed Sup in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            sw.Restart();
            using (StreamWriter file = File.CreateText(Path.Combine(MainPath, "bluraysup_example_out2.json")))
            {                
                serializer.Serialize(file, events);
            }
            sw.Stop();
            Console.WriteLine("Saved Sup JSON in {0} ms", sw.ElapsedMilliseconds);

        }
    }
}
