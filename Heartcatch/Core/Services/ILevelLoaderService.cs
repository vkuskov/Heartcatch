using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heartcatch.Core.Services
{
    public interface ILevelLoaderService
    {
        void LoadLevel(params LevelReference[] parts);
    }
}
