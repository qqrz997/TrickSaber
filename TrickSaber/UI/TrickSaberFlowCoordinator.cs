using HMUI;
using TrickSaber.ViewControllers;
using Zenject;

namespace TrickSaber.UI;

internal class TrickSaberFlowCoordinator : FlowCoordinator
{
    [Inject] private readonly MainFlowCoordinator _mainFlowCoordinator = null!;
    [Inject] private readonly BindingsViewController _bindingsViewController = null!;
    [Inject] private readonly MiscViewController _miscViewController = null!;
    [Inject] private readonly ThresholdViewController _thresholdViewController = null!;

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (firstActivation)
        {
            showBackButton = true;
            SetTitle("Tricksaber");
            ProvideInitialViewControllers(_bindingsViewController, _miscViewController, _thresholdViewController);
        }
    }

    protected override void BackButtonWasPressed(ViewController topViewController)
    {
        base.BackButtonWasPressed(topViewController);
        _mainFlowCoordinator.DismissFlowCoordinator(this);
    }
}