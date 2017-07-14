using UnityEngine;

namespace Heartcatch.Models
{
    [CreateAssetMenu(menuName = "Heartcatch/Game Config")]
    public sealed class GameConfigModel : ScriptableObject, IGameConfigModel
    {
        [SerializeField] private string gameName;
        [SerializeField] private string assetBundleUrl;
        [SerializeField] private string firstSceneBundle;
        [SerializeField] private bool isDevelopmentMode;
        [SerializeField] private bool isLocalBuild;

        public bool IsDevelopmentMode => isDevelopmentMode;

        public bool IsLocalBuild => isLocalBuild;

        public string AssetBundleUrl => assetBundleUrl;

        public string FirstSceneBundle => firstSceneBundle;

        public string GameName => gameName;
    }
}