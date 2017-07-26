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
            private double totalTime;

            public double TotalTime { get { return totalTime; } }
            public double DeltaTime { get { return deltaTime; } }

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

            public void Update(float deltaTime)
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
                totalTime += smoothDeltaTime;
                this.deltaTime = smoothDeltaTime;
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
            timeSmoother.Update(deltaTime);
            unscaledTimeSmoother.Update(unscaledDeltaTime);
            time = new GameTime()
            {
                DeltaTime = timeSmoother.DeltaTime,
                TotalTime = timeSmoother.TotalTime,
                UnscaledDeltaTime = unscaledTimeSmoother.DeltaTime,
                UnscaledTotalTime = unscaledTimeSmoother.TotalTime
            };
        }
    }
}