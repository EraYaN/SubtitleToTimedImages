using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace SubtitleToTimedImages
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var events = SubtitleUtil.ReadVobSub("E:\\out.sub", "E:\\out.idx");
            sw.Stop();
            Console.WriteLine("Parsed VobSub in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            sw.Restart();
            using (StreamWriter file = File.CreateText(@"E:\out.json"))
            {
                serializer.Serialize(file, events);
            }
            sw.Stop();
            Console.WriteLine("Saved VobSub JSON in {0} ms", sw.ElapsedMilliseconds);

            sw.Restart();
            events = SubtitleUtil.ReadSup("E:\\eng.sup");
            sw.Stop();
            Console.WriteLine("Parsed Sup in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            sw.Restart();
            using (StreamWriter file = File.CreateText(@"E:\eng.json"))
            {                
                serializer.Serialize(file, events);
            }
            sw.Stop();
            Console.WriteLine("Saved Sup JSON in {0} ms", sw.ElapsedMilliseconds);

        }
    }
}
