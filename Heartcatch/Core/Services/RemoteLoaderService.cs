namespace Heartcatch.Core.Services
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