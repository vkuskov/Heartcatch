using System;

namespace Heartcatch.Services
{
    public sealed class SmoothTimeService : ITimeService
    {
        private const int SAMPLES_COUNT = 15;
        private const int IGNORE_BORDER_CASES = 2;
        private const double DEFAULT_DELTA_TIME = 1.0 / 60.0;

        private readonly double[] samples = new double[SAMPLES_COUNT];
        private readonly double[] sortedSamples = new double[SAMPLES_COUNT];
        private int cursor;
        private double deltaTime;
        private double totlaElapsedTime;

        public SmoothTimeService()
        {
            Reset();
        }

        public GameTime Time
        {
            get
            {
                return new GameTime
                {
                    ElapsedTime = TimeSpan.FromSeconds(deltaTime),
                    TotalElapsedTime = TimeSpan.FromSeconds(totlaElapsedTime)
                };
            }
        }

        public void Reset()
        {
            for (var i = 0; i < SAMPLES_COUNT; ++i)
            {
                samples[i] = DEFAULT_DELTA_TIME;
                sortedSamples[i] = DEFAULT_DELTA_TIME;
            }
            deltaTime = DEFAULT_DELTA_TIME;
            cursor = 0;
        }

        public void Update(float deltaTime)
        {
            samples[cursor] = deltaTime;
            cursor++;
            cursor %= SAMPLES_COUNT;
            for (var i = 0; i < SAMPLES_COUNT; ++i)
                sortedSamples[i] = samples[i];
            Array.Sort(sortedSamples);
            var total = 0.0;
            for (var i = IGNORE_BORDER_CASES; i < SAMPLES_COUNT - IGNORE_BORDER_CASES; ++i)
                total += sortedSamples[i];
            var smoothDeltaTime = total / (SAMPLES_COUNT - IGNORE_BORDER_CASES * 2);
            this.deltaTime = smoothDeltaTime;
            totlaElapsedTime += smoothDeltaTime;
        }
    }
}