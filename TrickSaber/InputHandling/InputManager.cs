using System;
using System.Collections.Generic;
using System.Linq;
using TrickSaber.Configuration;
using UnityEngine.XR;

namespace TrickSaber.InputHandling;

internal class InputManager
{
    private readonly PluginConfig _config;
    private readonly TrickInputHandler _trickInputHandler = [];

    private InputManager(PluginConfig config)
    {
        _config = config;
    }

    public void Init(SaberType type)
    {
        var xrNode = type == SaberType.SaberA ? XRNode.LeftHand : XRNode.RightHand;

        var trigger = new TriggerHandler(_config.TriggerThreshold, _config.ReverseTrigger, xrNode);
        var grip = new GripHandler(_config.GripThreshold, _config.ReverseGrip, xrNode);
        var thumbstick = new ThumbstickHandler(_config.ThumbstickThreshold, _config.ReverseThumbstick, _config.ThumstickDirection, xrNode);
     
        _trickInputHandler.Add(_config.TriggerAction, trigger);
        _trickInputHandler.Add(_config.GripAction, grip);
        _trickInputHandler.Add(_config.ThumbstickAction, thumbstick);
    }
    
    public event Action<TrickAction, float>? TrickActivated;
    public event Action<TrickAction>? TrickDeactivated;

    public void Tick()
    {
        foreach (var (action, handlers) in _trickInputHandler)
        {
            if (CheckHandlersDown(handlers, out float val))
            {
                TrickActivated?.Invoke(action, val);
            }
            else if (CheckHandlersUp(handlers))
            {
                Plugin.Log.Notice($"{action} deactivated");
                TrickDeactivated?.Invoke(action);
            }
        }
    }

    private static bool CheckHandlersDown(HashSet<InputHandler> handlers, out float val)
    {
        val = 0;
        if (handlers.Count == 0) return false;
        bool output = true;
        foreach (var handler in handlers)
        {
            output &= handler.Activated(out float handlerValue);
            val += handlerValue;
        }

        if (output)
        {
            val /= handlers.Count;
        }

        return output;
    }

    private static bool CheckHandlersUp(HashSet<InputHandler> handlers)
    {
        foreach (var handler in handlers)
        {
            if (handler.Deactivated()) return true;
        }

        return false;
    }
}