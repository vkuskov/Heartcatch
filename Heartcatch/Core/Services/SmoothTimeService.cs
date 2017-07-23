using System;

namespace Heartcatch.Core.Services
{
    public sealed class SmoothTimeService : ITimeService
    {
        private sealed class TimeSmoother
        {
            private const int SamplesCount = 15;
            private const int IgnoreBorderCases = 2;
            private const double DefaultDeltaTime = 1.0 / 60.0;

            private readonly double[] samples = new double[SamplesCount];
            private readonly double[] sortedSamples = new double[SamplesCount];
            private int cursor;
            private double deltaTime;

            public void Reset()
            {
                for (var i = 0; i < SamplesCount; ++i)
                {
                    samples[i] = DefaultDeltaTime;
                    sortedSamples[i] = DefaultDeltaTime;
                }
                deltaTime = DefaultDeltaTime;
                cursor = 0;
            }

            public double Update(float deltaTime)
            {
                samples[cursor] = deltaTime;
                cursor++;
                cursor %= SamplesCount;
                for (var i = 0; i < SamplesCount; ++i)
                    sortedSamples[i] = samples[i];
                Array.Sort(sortedSamples);
                var total = 0.0;
                for (var i = IgnoreBorderCases; i < SamplesCount - IgnoreBorderCases; ++i)
                    total += sortedSamples[i];
                var smoothDeltaTime = total / (SamplesCount - IgnoreBorderCases * 2);
                return smoothDeltaTime;
            }
        }

        private TimeSmoother timeSmoother = new TimeSmoother();
        private TimeSmoother unscaledTimeSmoother = new TimeSmoother();
        private GameTime time;

        public SmoothTimeService()
        {
            Reset();
        }

        public GameTime Time
        {
            get { return time; }
        }

        public void Reset()
        {
            timeSmoother.Reset();
            unscaledTimeSmoother.Reset();
        }

        internal void Update(float deltaTime, float unscaledDeltaTime)
        {
            time.DeltaTime = timeSmoother.Update(deltaTime);
            time.UnscaledDeltaTime = unscaledTimeSmoother.Update(unscaledDeltaTime);
        }
    }
}