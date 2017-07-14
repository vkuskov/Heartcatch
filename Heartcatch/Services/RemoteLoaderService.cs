using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartcatch.Services
{
    public sealed class RemoteLoaderService : BaseLoaderService
    {
        private readonly string baseUrl;

        public RemoteLoaderService(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        protected override IAssetLoaderFactory CreateAssetLoaderFactory()
        {
            return new WebAssetLoaderFactory(this, baseUrl);
        }
    }
}
