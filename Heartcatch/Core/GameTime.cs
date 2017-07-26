using System;

namespace Heartcatch.Core
{
    [Serializable]
    public struct GameTime
    {
        public double DeltaTime;
        public double UnscaledDeltaTime;
        public double TotalTime;
        public double UnscaledTotalTime;
    }
}