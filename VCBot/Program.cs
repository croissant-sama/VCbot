using OpenCvSharp;
using System;
using System.Drawing;
using VCBot.Properties;

namespace VCBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();

            Program program = new Program(
                new MouseService(),
                new DelayService(random),
                new ScreenshotService(),
                new Logger(),
                random
            );
            program.Run();

            Console.WriteLine("Программа завершена");
            Console.ReadKey();
        }
        
        private MouseService MouseService;
        private DelayService DelayService;
        private ScreenshotService ScreenshotService;
        private Logger Logger;
        private Random Random;

        public Program(
            MouseService mouseService,
            DelayService delayService,
            ScreenshotService screenshotService,
            Logger logger,
            Random random
        )
        {
            MouseService = mouseService;
            DelayService = delayService;
            ScreenshotService = screenshotService;
            Logger = logger;
            Random = random;
        }

        public void Run()
        {
            while (true)
            {
                Bitmap screenshot = ScreenshotService.Do();

                double delta = 0.9;
                int attemps = 5;

                Action<OpenCvSharp.Point, int, int> clickAction =
                    (point, width, height) => Click(point, width, height);
                Action<OpenCvSharp.Point, int, int> clickUpAction =
                    (point, width, height) => ClickUp(point, width, height);
                Action<OpenCvSharp.Point, int, int> clickErrorAction =
                    (point, width, height) => ClickError(point, width, height);

                if (TryToClick(Resources.error, clickErrorAction, delta, "error"))
                {
                    continue;
                }

                if (TryToClick(Resources.ok, clickAction, delta, "ok"))
                {
                    continue;
                }

                if (TryToClick(Resources.up, clickUpAction, delta, "up"))
                {
                    while (TryToClick(Resources.up, clickUpAction, delta, "up")) ;
                    continue;
                }
                
                if (TryToClick(Resources.tech1, clickAction, delta, "tech1"))
                {
                    TryToClickGold(delta, attemps);
                    continue;
                }

                if (TryToClick(Resources.tech2, clickAction, delta, "tech2"))
                {
                    TryToClickGold(delta, attemps);
                    continue;
                }

                if (TryToClick(Resources.tech3, clickAction, delta, "tech3"))
                {
                    TryToClickGold(0.9, attemps);
                    continue;
                }

                if (TryToClick(Resources.tech4, clickAction, delta, "tech4"))
                {
                    TryToClickGold(delta, attemps);
                    continue;
                }

                if (TryToClick(Resources.tech5, clickAction, delta, "tech5"))
                {
                    TryToClickGold(delta, attemps);
                    continue;
                }

                DelayService.RandomSleep(1, 15);
            }
        }
        
        private bool TryToClick(Bitmap resource, Action<OpenCvSharp.Point, int, int> clickAction, double delta, string step)
        {
            Bitmap screenshot = ScreenshotService.Do();

            double maxVal;
            OpenCvSharp.Point maxLoc;

            Match(resource, screenshot, out maxVal, out maxLoc);
            Logger.Log(step + ": " + maxVal + " " + maxLoc);

            if (maxVal < delta)
            {
                return false;
            }

            Logger.Log(screenshot);
            clickAction(maxLoc, resource.Width, resource.Height);

            return true;
        }
        
        private bool TryToClickGold(double delta, int attemps)
        {
            Bitmap resource = Resources.gold;

            double maxVal;
            OpenCvSharp.Point maxLoc;

            for(int i = 0; i < attemps; i++)
            {
                Bitmap screenshot = ScreenshotService.Do();

                Match(resource, screenshot, out maxVal, out maxLoc);
                Logger.Log("gold: " + maxVal + " " + maxLoc);

                if (maxVal >= delta)
                {
                    Click(maxLoc, resource.Width, resource.Height);
                    return true;
                }

                DelayService.RandomSleep(1, 2);
            }

            return false;
        }

        private void ClickError(OpenCvSharp.Point maxLoc, int width, int height)
        {
            Logger.Log("Error!");
        }

        private void ClickUp(OpenCvSharp.Point maxLoc, int width, int height)
        {
            MouseService.ClickRight(
                maxLoc.X + width + Random.Next(0, width - 1),
                maxLoc.Y + Random.Next(0, height - 1)
            );
            Logger.Log("Up!");
        }

        private void Click(OpenCvSharp.Point maxLoc, int width, int height)
        {
            MouseService.ClickRight(
                maxLoc.X + Random.Next(0, width - 1),
                maxLoc.Y + Random.Next(0, height - 1)
            );
            Logger.Log("Click!");
        }

        private void Match(Bitmap screen, Bitmap resource, out double maxVal, out OpenCvSharp.Point maxLoc)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            Mat source = OpenCvSharp.Extensions.BitmapConverter.ToMat(screen);
            Mat template = OpenCvSharp.Extensions.BitmapConverter.ToMat(resource);

            Mat result = source.MatchTemplate(template, TemplateMatchModes.CCoeffNormed);

            double minVal;
            OpenCvSharp.Point minLoc;

            result.MinMaxLoc(out minVal, out maxVal, out minLoc, out maxLoc);
        }
    }
}
