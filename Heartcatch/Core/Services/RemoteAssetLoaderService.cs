namespace Heartcatch.Core.Services
{
    public sealed class RemoteAssetLoaderService : BaseAssetLoaderService
    {
        private readonly string baseUrl;

        public RemoteAssetLoaderService(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        protected override IAssetLoaderFactory CreateAssetLoaderFactory()
        {
            return new WebAssetLoaderFactory(this, baseUrl);
        }
    }
}