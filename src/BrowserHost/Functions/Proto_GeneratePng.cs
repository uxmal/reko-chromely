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
        public static void Execute(PromiseTask promise)
        {
            using var bmp = new Bitmap(300, 30);
            using var font = new Font("Arial", 7.0F);
            using var g = Graphics.FromImage(bmp);
            using var bg = new SolidBrush(Color.FromArgb(unchecked((int)0xFF80E080)));
            using var fg = new SolidBrush(Color.FromArgb(unchecked((int)0xFF101020)));
            g.FillRectangle(bg, 0, 0, 300, 30);
            g.DrawString("Reko", font, fg, new PointF(3, 3));
            //g.FillRectangle(Brushes.Red, 3, 3, 20, 20);
            using var mem = new MemoryStream();
            bmp.Save(mem, ImageFormat.Png);
            mem.Flush();
            var bytes = mem.ToArray();
            promise.Resolve(bytes);
        }
    }
}
