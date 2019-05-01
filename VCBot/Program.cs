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
        }
        
        private MouseService MouseService;
        private DelayService DelayService;
        private ScreenshotService ScreenshotService;
        private Logger Logger;
        private Random Random;

        private Action<OpenCvSharp.Point, int, int> ClickAction;
        private Action<OpenCvSharp.Point, int, int> ClickUpAction;
        private Action<OpenCvSharp.Point, int, int> ClickErrorAction;

        private double Delta = 0.9;
        private int Attemps = 5;

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

            ClickAction = (point, width, height) => Click(point, width, height);
            ClickUpAction = (point, width, height) => ClickUp(point, width, height);
            ClickErrorAction = (point, width, height) => ClickError(point, width, height);
        }

        public void Run()
        {
            while (true)
            {
                Bitmap screenshot = ScreenshotService.Do();
                
                if (TryToClick(screenshot, Resources.error, ClickErrorAction, "error"))
                {
                    continue;
                }

                if (TryToClick(screenshot, Resources.cancel, ClickAction, "cancel"))
                {
                    continue;
                }

                if (TryToClick(screenshot, Resources.ok, ClickAction, "ok"))
                {
                    continue;
                }

                if (TryToClick(screenshot, Resources.up, ClickUpAction, "up"))
                {
                    while (TryToClick(ScreenshotService.Do(), Resources.up, ClickUpAction, "up")) ;
                    continue;
                }
                
                if (TryToClick(screenshot, Resources.tech1, ClickAction, "tech1"))
                {
                    TryToClickGold();
                    continue;
                }

                if (TryToClick(screenshot, Resources.tech2, ClickAction, "tech2"))
                {
                    TryToClickGold();
                    continue;
                }

                if (TryToClick(screenshot, Resources.tech3, ClickAction, "tech3"))
                {
                    TryToClickGold();
                    continue;
                }

                if (TryToClick(screenshot, Resources.tech4, ClickAction, "tech4"))
                {
                    TryToClickGold();
                    continue;
                }

                if (TryToClick(screenshot, Resources.tech5, ClickAction, "tech5"))
                {
                    TryToClickGold();
                    continue;
                }

                DelayService.RandomSleep(1, 15);
            }
        }
        
        private bool TryToClick(Bitmap screenshot, Bitmap resource, Action<OpenCvSharp.Point, int, int> clickAction, string step)
        {
            double maxVal;
            OpenCvSharp.Point maxLoc;

            Match(resource, screenshot, out maxVal, out maxLoc);
            Logger.Log(step + ": " + maxVal + " " + maxLoc);

            if (maxVal < Delta)
            {
                return false;
            }

            Logger.Log(screenshot);
            clickAction(maxLoc, resource.Width, resource.Height);

            return true;
        }
        
        private bool TryToClickGold()
        {
            for(int i = 0; i < Attemps; i++)
            {
                if (TryToClick(ScreenshotService.Do(), Resources.gold, ClickAction, "gold"))
                {
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
