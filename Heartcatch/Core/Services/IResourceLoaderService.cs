﻿using Heartcatch.Core.Models;

namespace Heartcatch.Core.Services
{
    public interface IResourceLoaderService
    {
        void RequestResources(IResourceModel resourceModel);
    }
}