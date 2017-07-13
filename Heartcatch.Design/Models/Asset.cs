using System;
using Object = UnityEngine.Object;

namespace Heartcatch.Design.Models
{
    [Serializable]
    public struct Asset
    {
        public string Name;
        public Object HiDefAsset;
    }
}