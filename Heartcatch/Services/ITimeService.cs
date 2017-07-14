namespace Heartcatch.Services
{
    public interface ITimeService
    {
        GameTime Time { get; }
        void Reset();
    }
}