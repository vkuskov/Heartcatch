using UnityEngine;

namespace Heartcatch.Models
{
    [CreateAssetMenu(menuName = "Heartcatch/Game Config")]
    public sealed class GameConfigModel : ScriptableObject, IGameConfigModel
    {
        [SerializeField] private string assetBundleUrl;

        [SerializeField] private string firstSceneBundle;

        public string AssetBundleURL
        {
            get { return assetBundleUrl; }
        }

        public string FirstSceneBundle
        {
            get { return firstSceneBundle; }
        }
    }
}