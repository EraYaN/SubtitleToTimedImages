using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace SubtileToTimedImages
{
    static class ImageUtil
    {
        // In jellyfin we should probably use existing skia stuff or something similar.
        public static byte[] ConvertToPngBytes(Image img)
        {
            using var stream = new MemoryStream();            
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }
}
