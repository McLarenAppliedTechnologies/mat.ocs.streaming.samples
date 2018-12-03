using System;

namespace MAT.OCS.Streaming.Samples.Samples
{
    class RandomRangeWalker
    {
        private readonly double min;
        private readonly double max;
        private readonly Random random;
        private double value;

        public RandomRangeWalker(double min, double max)
        {
            this.min = min;
            this.max = max;
            this.random = new Random();
        }

        public double GetNext()
        {
            var nextChange = (random.NextDouble() - 0.5) / 4;
            if (nextChange + value < min || nextChange + value > max)
                nextChange = -nextChange;
            value += nextChange;
            return value;
        }
    }
}
