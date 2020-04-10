using System;
using System.Drawing;

namespace SubtitleToTimedImages
{
    class SubtitleEvent
    {
        public TimeSpan StartTime;
        public TimeSpan EndTime;
        // MimeType and Imahe byte data will let up compose the data URI client side.
        public string MimeType;
        public byte[] Image;
        public Point Origin;
    }
}
