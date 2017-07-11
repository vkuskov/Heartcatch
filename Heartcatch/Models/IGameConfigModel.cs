namespace Heartcatch.Models
{
    public interface IGameConfigModel
    {
        bool IsDevelopmentMode { get; }
        string AssetBundleUrl { get; }
        string FirstSceneBundle { get; }
    }
}