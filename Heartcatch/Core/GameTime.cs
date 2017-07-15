using System;

namespace Heartcatch.Core
{
    [Serializable]
    public struct GameTime
    {
        public TimeSpan ElapsedTime;
        public TimeSpan TotalElapsedTime;
    }
}