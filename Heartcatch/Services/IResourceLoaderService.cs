using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heartcatch.Models;

namespace Heartcatch.Services
{
    public interface IResourceLoaderService
    {
        void RequestResources(IResourceModel resourceModel);
    }
}
