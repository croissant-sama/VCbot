using OpenCvSharp;
using System;
using System.Drawing;
using System.Threading;
using VCBot.Properties;

namespace VCBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program(
                new MouseService(),
                new ScreenshotService(),
                new Random()
            );
            program.Run();
        }

        private const double MATH_TEMPLATE_DELTA = 0.9;

        private MouseService MouseService;
        private ScreenshotService ScreenshotService;
        private Random Random;
        
        public Program(
            MouseService mouseService,
            ScreenshotService screenshotService, 
            Random random
        )
        {
            MouseService = mouseService;
            ScreenshotService = screenshotService;
            Random = random;
        }

        public void Run()
        {
            while (true)
            {
                if (TryToClick("error"))
                {
                    return;
                }

                TryToAnyClicks();
                
                Thread.Sleep(1000 + Random.Next(0, 15 * 1000));
            }
        }

        public void TryToAnyClicks()
        {
            if (TryToClick("ok"))
            {
                return;
            }

            if (TryToClick("up"))
            {
                while (TryToClick("up"));
                return;
            }

            if (TryToClick("tech1"))
            {
                while (TryToClick("gold"));
                return;
            }

            if (TryToClick("tech2"))
            {
                while (TryToClick("gold")) ;
                return;
            }

            if (TryToClick("tech3"))
            {
                while (TryToClick("gold")) ;
                return;
            }

            if (TryToClick("tech4"))
            {
                while (TryToClick("gold")) ;
                return;
            }

            if (TryToClick("tech5"))
            {
                while (TryToClick("gold")) ;
                return;
            }
        }
        
        private bool TryToClick(string resName)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Bitmap res = (Bitmap)Resources.ResourceManager.GetObject(resName);

            Mat source = OpenCvSharp.Extensions.BitmapConverter.ToMat(ScreenshotService.Do());
            Mat template = OpenCvSharp.Extensions.BitmapConverter.ToMat(res);

            Mat result = source.MatchTemplate(template, TemplateMatchModes.CCoeffNormed);
            
            double minVal, maxVal;
            OpenCvSharp.Point minLoc, maxLoc;

            result.MinMaxLoc(out minVal, out maxVal, out minLoc, out maxLoc);
            
            Console.WriteLine(resName + ": " + maxVal + " " + maxLoc);

            if (maxVal < MATH_TEMPLATE_DELTA)
            {
                return false;
            }

            Click(maxLoc, resName, res);

            return true;
        }

        private void Click(OpenCvSharp.Point maxLoc, string resName, Bitmap res)
        {
            if (resName.Equals("error"))
            {
                return;
            }

            if (resName.Equals("up"))
            {
                MouseService.ClickRight(
                    maxLoc.X + res.Size.Width + Random.Next(0, res.Size.Width - 1),
                    maxLoc.Y + Random.Next(0, res.Size.Height - 1)
                );
                return;
            }

            MouseService.ClickRight(
                maxLoc.X + Random.Next(0, res.Size.Width - 1),
                maxLoc.Y + Random.Next(0, res.Size.Height - 1)
            );
        }
    }
}
