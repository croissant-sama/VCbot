using System.Drawing;
using System.Windows.Forms;

namespace VCBot
{
    class ScreenshotService
    {
        public Bitmap Do()
        {
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;
            Bitmap screenshot = new Bitmap(width, height);

            using (var g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                return screenshot;
            }
        }
    }
}
