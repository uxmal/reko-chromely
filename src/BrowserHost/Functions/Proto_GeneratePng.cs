using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Reko.Chromely.BrowserHost.Functions
{
    public class Proto_GeneratePng
    {
        private static Color FromArgb(uint n)
        {
            return Color.FromArgb(unchecked((int)n));
        }

        public static byte[] Generate(int percentage)
        {
            using var bmp = new Bitmap(300, 30);
            using var font = new Font("Arial", 7.0F);
            using var g = Graphics.FromImage(bmp);
            using var bg = new SolidBrush(FromArgb(0xFF80E080u));
            using var fg = new SolidBrush(FromArgb(0xFF101020u));
            using var un = new SolidBrush(FromArgb(0xFFCCCCCCu));
            var boundary = 300.0F * percentage / 100F;
            g.FillRectangle(bg, 0, 0, boundary, 30);
            g.FillRectangle(un, boundary, 0, 300, 30);
            g.DrawString("Reko", font, fg, new PointF(3, 3));
            using var mem = new MemoryStream();
            bmp.Save(mem, ImageFormat.Png);
            mem.Flush();
            var bytes = mem.ToArray();
            return bytes;
        }

        public static void Execute(PromiseTask promise)
        {
            var bytes = Generate(100);
            promise.Resolve(bytes);
        }
    }
}
