using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using Zenject;

namespace TrickSaber.UI;

internal class SettingsUI : IInitializable, IDisposable
{
    private readonly TrickSaberFlowCoordinator _trickSaberFlowCoordinator;
    private readonly MenuButton _menuButton;

    private SettingsUI(TrickSaberFlowCoordinator trickSaberFlowCoordinator)
    {
        _trickSaberFlowCoordinator = trickSaberFlowCoordinator;
        _menuButton = new MenuButton("Trick Saber", "Change your tricks!", ShowFlow);
    }

    public void Initialize()
    {
        MenuButtons.Instance.RegisterButton(_menuButton);
    }

    private void ShowFlow()
    {
        BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_trickSaberFlowCoordinator);
    }

    public void Dispose()
    {
        MenuButtons.Instance.UnregisterButton(_menuButton);
    }
}