using System;
using System.Collections.Generic;
using TrickSaber.Configuration;
using UnityEngine.XR;

namespace TrickSaber.InputHandling;

internal class InputManager
{
    public event Action<TrickAction, float> TrickActivated;
    public event Action<TrickAction> TrickDeactivated;

    private readonly PluginConfig _config;
    private readonly TrickInputHandler _trickInputHandler;

    private InputManager(PluginConfig config)
    {
        _config = config;

        _trickInputHandler = new TrickInputHandler();
    }

    public void Init(SaberType type)
    {
        XRNode node = type == SaberType.SaberA ? XRNode.LeftHand : XRNode.RightHand;
        var controllerInputDevice = InputDevices.GetDeviceAtXRNode(node);

        var dir = _config.ThumstickDirection;

        var triggerHandler = new TriggerHandler(node, _config.TriggerThreshold, _config.ReverseTrigger);
        var gripHandler = new GripHandler(controllerInputDevice, _config.GripThreshold, _config.ReverseGrip);
        var thumbstickAction = new ThumbstickHandler(node, _config.ThumbstickThreshold, dir, _config.ReverseThumbstick);

        _trickInputHandler.Add(_config.TriggerAction, triggerHandler);
        _trickInputHandler.Add(_config.GripAction, gripHandler);
        _trickInputHandler.Add(_config.ThumbstickAction, thumbstickAction);
    }

    // Using ITickable seems to result in GetHandlers returning no handlers (?!?)
    // So we need to manually tick
    public void Tick()
    {
        foreach (TrickAction trickAction in _trickInputHandler.TrickHandlerSets.Keys)
        {
            var handlers = _trickInputHandler.GetHandlers(trickAction);
            if (CheckHandlersDown(handlers, out var val))
                TrickActivated?.Invoke(trickAction, val);

            else if (CheckHandlersUp(handlers)) TrickDeactivated?.Invoke(trickAction);
        }
    }

    private bool CheckHandlersDown(ISet<InputHandler> handlers, out float val)
    {
        val = 0;
        if (handlers.Count == 0) return false;
        bool output = true;
        foreach (var handler in handlers)
        {
            output &= handler.Activated(out var handlerValue);
            val += handlerValue;
        }

        if (output) val /= handlers.Count;

        return output;
    }

    private bool CheckHandlersUp(ISet<InputHandler> handlers)
    {
        foreach (InputHandler handler in handlers)
            if (handler.Deactivated())
                return true;

        return false;
    }
}