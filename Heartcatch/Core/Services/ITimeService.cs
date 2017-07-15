namespace Heartcatch.Core.Services
{
    public interface ITimeService
    {
        GameTime Time { get; }
        void Reset();
    }
}