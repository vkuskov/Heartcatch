using System;

namespace Heartcatch.Core.Services
{
    public sealed class SmoothTimeService : ITimeService
    {
        private const int SamplesCount = 15;
        private const int IgnoreBorderCases = 2;
        private const double DefaultDeltaTime = 1.0 / 60.0;

        private readonly double[] samples = new double[SamplesCount];
        private readonly double[] sortedSamples = new double[SamplesCount];
        private int cursor;
        private double deltaTime;
        private double totlaElapsedTime;

        public SmoothTimeService()
        {
            Reset();
        }

        public GameTime Time => new GameTime
        {
            ElapsedTime = TimeSpan.FromSeconds(deltaTime),
            TotalElapsedTime = TimeSpan.FromSeconds(totlaElapsedTime)
        };

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
            this.deltaTime = smoothDeltaTime;
            totlaElapsedTime += smoothDeltaTime;
        }
    }
}