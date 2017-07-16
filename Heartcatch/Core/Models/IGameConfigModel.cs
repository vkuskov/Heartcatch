namespace Heartcatch.Core.Models
{
    public interface IGameConfigModel
    {
        bool IsDevelopmentMode { get; }
        bool IsLocalBuild { get; }
        string LoadingScene { get; }
        string AssetBundleUrl { get; }
        LevelReference FirstLevel { get; }
        string GameName { get; }
    }
}