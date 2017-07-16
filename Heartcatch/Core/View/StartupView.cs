using UnityEngine;

namespace Heartcatch.Core.View
{
    public sealed class StartupView : strange.extensions.mediation.impl.View
    {
        [SerializeField] private string[] preloadedBundles;

        public string[] PreloadedBundles { get { return preloadedBundles; } }
    }
}
