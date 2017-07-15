namespace Heartcatch.Core.Models
{
    public interface IGameConfigModel
    {
        bool IsDevelopmentMode { get; }
        bool IsLocalBuild { get; }
        string AssetBundleUrl { get; }
        string FirstSceneBundle { get; }
        string GameName { get; }
    }
}