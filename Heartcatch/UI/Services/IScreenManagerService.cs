using Heartcatch.Core;
using Heartcatch.UI.Models;

namespace Heartcatch.UI.Services
{
    public interface IScreenManagerService
    {
        void AddScreen(IScreenModel screen);
        void RemoveScreen(IScreenModel screen);
        void Update(GameTime gameTime);
    }
}