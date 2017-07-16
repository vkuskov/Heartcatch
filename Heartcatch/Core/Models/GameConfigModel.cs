using UnityEngine;

namespace Heartcatch.Core.Models
{
    [CreateAssetMenu(menuName = "Heartcatch/Game Config")]
    public sealed class GameConfigModel : ScriptableObject, IGameConfigModel
    {
        [SerializeField] private string assetBundleUrl;
        [SerializeField] private string gameName;
        [SerializeField] private bool isDevelopmentMode;
        [SerializeField] private bool isLocalBuild;
        [SerializeField] private string loadingScene;
        [SerializeField] private LevelReference firstLevel;

        public bool IsDevelopmentMode
        {
            get { return isDevelopmentMode; }
        }

        public bool IsLocalBuild
        {
            get { return isLocalBuild; }
        }

        public string LoadingScene
        {
            get { return loadingScene; }
        }

        public string AssetBundleUrl
        {
            get { return assetBundleUrl; }
        }

        public LevelReference FirstLevel
        {
            get { return firstLevel; }
        }

        public string GameName
        {
            get { return gameName; }
        }
    }
}