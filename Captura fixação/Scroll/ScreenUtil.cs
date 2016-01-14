using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Scroll
{
    public static class ScreenUtil
    {
        public static Bitmap Capture(int x, int y, int width, int height/*, int xx, int yy*/)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(x, y, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);

              
            }
            return bitmap;
        }
    }
}
