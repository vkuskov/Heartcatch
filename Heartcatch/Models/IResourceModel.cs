using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartcatch.Models
{
    public interface IResourceModel
    {
        void CollectResources(IResourceRequestModel request);
        void OnResourcesLoaded(IResourceRequestModel request);
    }
}
