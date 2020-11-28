using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace SubtitleToTimedImages
{
    class Program
    {

        const string MainPath = @"E:\";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };

            Stopwatch sw = new Stopwatch();
            //sw.Start();
            //var events = SubtitleUtil.ReadMatroskaVobSub(Path.Combine(MainPath,"out.mkv"),0);
            //sw.Stop();
            //Console.WriteLine("Parsed Matroska VobSub in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            //sw.Restart();
            //using (StreamWriter file = File.CreateText(Path.Combine(MainPath, "out_mkv.json")))
            //{
            //    serializer.Serialize(file, events);
            //}
            //sw.Stop();
            //Console.WriteLine("Saved Matroska VobSub JSON in {0} ms", sw.ElapsedMilliseconds);

            //sw.Restart();
            var events = SubtitleUtil.ReadMatroskaBluraySup(Path.Combine(MainPath, "eng.mkv"), 0);
            sw.Stop();
            Console.WriteLine("Parsed Matroska Sup in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            sw.Restart();
            using (StreamWriter file = File.CreateText(Path.Combine(MainPath, "eng_mkv.json")))
            {
                serializer.Serialize(file, events);
            }
            sw.Stop();
            Console.WriteLine("Saved Matroska Sup JSON in {0} ms", sw.ElapsedMilliseconds);

            //sw.Restart();            
            //events = SubtitleUtil.ReadVobSub(Path.Combine(MainPath, "out.sub"), Path.Combine(MainPath, "out.idx"));
            //sw.Stop();
            //Console.WriteLine("Parsed VobSub in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            //sw.Restart();
            //using (StreamWriter file = File.CreateText(Path.Combine(MainPath, "out.json")))
            //{
            //    serializer.Serialize(file, events);
            //}
            //sw.Stop();
            //Console.WriteLine("Saved VobSub JSON in {0} ms", sw.ElapsedMilliseconds);

            sw.Restart();
            events = SubtitleUtil.ReadSup(Path.Combine(MainPath, "eng.sup"));
            sw.Stop();
            Console.WriteLine("Parsed Sup in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            sw.Restart();
            using (StreamWriter file = File.CreateText(Path.Combine(MainPath, "eng.json")))
            {                
                serializer.Serialize(file, events);
            }
            sw.Stop();
            Console.WriteLine("Saved Sup JSON in {0} ms", sw.ElapsedMilliseconds);

        }
    }
}
