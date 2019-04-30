using System;
using System.Threading;

namespace VCBot
{
    class DelayService
    {
        private Random Random;

        public DelayService(Random random)
        {
            Random = random;
        }

        public void RandomSleep(int minSec, int maxSec)
        {
            Thread.Sleep(minSec * 1000 + Random.Next(0, maxSec * 1000));
        }
    }
}
