using Heartcatch.Core;
using Heartcatch.UI.Services;

namespace Heartcatch.UI.Models
{
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden
    }

    public interface IScreenModel
    {
        bool IsPopup { get; }
        ScreenState State { get; }
        float TransitionPosition { get; }
        void Attach(IScreenManagerService screenManagerService);
        void OnUpdate(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen);
        void ExitScreen();
    }
}