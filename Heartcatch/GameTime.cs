using System;

namespace Heartcatch
{
    [Serializable]
    public struct GameTime
    {
        public TimeSpan ElapsedTime;
        public TimeSpan TotalElapsedTime;
    }
}