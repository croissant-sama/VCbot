using System;
using System.Drawing;

namespace VCBot
{
    class Logger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Log(Bitmap image)
        {
            image.Save(DateTime.Now.ToString("yyyyMMddHHmmss") + ".png" );
        }
    }
}
