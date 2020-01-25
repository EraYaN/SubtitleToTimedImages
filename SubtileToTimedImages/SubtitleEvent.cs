using System;
using System.Drawing;

namespace SubtileToTimedImages
{
    class SubtitleEvent
    {
        public TimeSpan StartTime;
        public TimeSpan EndTime;
        public string MimeType;
        public byte[] Image;
        public Point Origin;
    }
}
