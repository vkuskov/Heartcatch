using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Heartcatch.Models
{
    public interface IResourceRequestModel
    {
        void RequestResource(string name, AssetReference assetReference);
        Object GetResource(string name);
    }
}
