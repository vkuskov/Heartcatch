namespace Heartcatch.Core.Services
{
    public interface ILevelLoaderService
    {
        void LoadScenes(params string[] paths);
        void Update();
    }
}