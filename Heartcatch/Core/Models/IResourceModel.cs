namespace Heartcatch.Core.Models
{
    public interface IResourceModel
    {
        void CollectResources(IResourceRequestModel request);
        void OnResourcesLoaded(IResourceRequestModel request);
    }
}