using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Gma.System.MouseKeyHook
{
    public static class ScreenUtil
    {
        public static Bitmap Capture(int x, int y, int width, int height, int xx, int yy)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(x, y, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);

                User32.CURSORINFO cursorInfo;
                cursorInfo.cbSize = Marshal.SizeOf(typeof(User32.CURSORINFO));

                if (User32.GetCursorInfo(out cursorInfo))
                {
                    // if the cursor is showing draw it on the screen shot
                    if (cursorInfo.flags == User32.CURSOR_SHOWING)
                    {
                        var iconPointer = User32.CopyIcon(cursorInfo.hCursor);
                        User32.ICONINFO iconInfo;
                        int iconX, iconY;

                        if (User32.GetIconInfo(iconPointer, out iconInfo))
                        {
                            // calculate the correct position of the cursor
                            //iconX = cursorInfo.ptScreenPos.x -((int)iconInfo.xHotspot);
                            //iconY = cursorInfo.ptScreenPos.y -((int)iconInfo.yHotspot);

                            iconX = xx;
                            iconY = yy;

                            // draw the cursor icon on top of the captured screen image
                            User32.DrawIcon(g.GetHdc(), iconX, iconY, cursorInfo.hCursor);

                            // release the handle created by call to g.GetHdc()
                            g.ReleaseHdc();
                        }
                    }
                }
            }
            return bitmap;
        }
    }
}
