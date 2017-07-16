namespace Heartcatch.Core.Services
{
    public interface ISceneLoaderService
    {
        void LoadScenes(params string[] paths);
        void Update();
    }
}