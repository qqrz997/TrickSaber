using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using TrickSaber.Configuration;
using Zenject;

namespace TrickSaber.ViewControllers;

internal class BindingsViewController : BSMLResourceViewController
{
    public Array TrickActionOptions = Enum.GetValues(typeof(TrickAction));

    public override string ResourceName => "TrickSaber.Views.BindingsView.bsml";

    [Inject] private readonly PluginConfig _config = null!;

    public TrickAction TriggerAction
    {
        get => _config.TriggerAction;
        set
        {
            _config.TriggerAction = value;
            CheckMultiBinding();
        }
    }

    public TrickAction GripAction
    {
        get => _config.GripAction;
        set
        {
            _config.GripAction = value;
            CheckMultiBinding();
        }
    }

    public TrickAction ThumbAction
    {
        get => _config.ThumbstickAction;
        set
        {
            _config.ThumbstickAction = value;
            CheckMultiBinding();
        }
    }

    public bool ReverseTrigger
    {
        get => _config.ReverseTrigger;
        set => _config.ReverseTrigger = value;
    }

    public bool ReverseGrip
    {
        get => _config.ReverseGrip;
        set => _config.ReverseGrip = value;
    }

    public bool ReverseThumbstick
    {
        get => _config.ReverseThumbstick;
        set => _config.ReverseThumbstick = value;
    }

    public string Version => Plugin.Metadata.HVersion.ToString();
    public string Credits => "Original mod by Toni Macaroni";

    private bool _multiBindingTextActive;
    public bool MultiBindingTextActive
    {
        get => _multiBindingTextActive;
        set
        {
            _multiBindingTextActive = value;
            NotifyPropertyChanged();
        }
    }

    private void CheckMultiBinding()
    {
        MultiBindingTextActive = new List<TrickAction> { TriggerAction, GripAction, ThumbAction }
            .Where(action => action != TrickAction.None)
            .GroupBy(action => action)
            .Any(group => group.Count() > 1);
    }
}