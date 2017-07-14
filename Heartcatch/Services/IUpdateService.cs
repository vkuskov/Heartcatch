namespace Heartcatch.Services
{
    public interface IUpdateService
    {
        void Subscribe(IUpdateable updateable, int priority);
        void Unsubscribe(IUpdateable updateable);
    }
}