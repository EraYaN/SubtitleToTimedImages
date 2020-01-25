using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace SubtileToTimedImages
{
    static class ImageUtil
    {
        public static byte[] ConvertToPngBytes(Image img)
        {
            using var stream = new MemoryStream();            
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }
}
