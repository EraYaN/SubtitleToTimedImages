using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace SubtileToTimedImages
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var events = SubtitleUtil.ReadVobSub("E:\\out.sub", "E:\\out.idx");
            sw.Stop();
            Console.WriteLine("Parsed VobSub in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            sw.Reset();
            sw.Start();
            using (StreamWriter file = File.CreateText(@"E:\out.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, events);
            }
            sw.Stop();
            Console.WriteLine("Saved VobSub JSON in {0} ms", sw.ElapsedMilliseconds);

            sw.Reset();
            sw.Start();
            events = SubtitleUtil.ReadSup("E:\\eng.sup");
            sw.Stop();
            Console.WriteLine("Parsed Sup in {0} ms for {1} items", sw.ElapsedMilliseconds, events.Count);

            sw.Reset();
            sw.Start();
            using (StreamWriter file = File.CreateText(@"E:\eng.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, events);
            }
            sw.Stop();
            Console.WriteLine("Saved Sup JSON in {0} ms", sw.ElapsedMilliseconds);

        }
    }
}
