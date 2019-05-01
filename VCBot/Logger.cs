using System;
using System.Drawing;
using VCBot.Properties;

namespace VCBot
{
    class Logger
    {
        public void Log(string message)
        {
            if (Settings.Default.LoggingMessage)
            {
                Console.WriteLine(message);
            }
        }

        public void Log(Bitmap image)
        {
            if (Settings.Default.LoggingImage)
            {
                image.Save(DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
            }
        }
    }
}
