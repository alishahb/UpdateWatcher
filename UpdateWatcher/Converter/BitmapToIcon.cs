using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Alisha.UpdateWatcher.Converter
{

    class BitmapToIcon
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        public static Icon Convert(Bitmap bitmap)
        {
            var Hicon = bitmap.GetHicon();
            var icon = Icon.FromHandle(Hicon);

            DestroyIcon(Hicon);
            return icon;
        }
    }
}
