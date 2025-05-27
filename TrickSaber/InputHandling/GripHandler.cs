using UnityEngine.XR;

namespace TrickSaber.InputHandling;

internal class GripHandler : InputHandler
{
    private InputDevice _controllerInputDevice;

    public GripHandler(
        float threshold,
        bool isReversed,
        XRNode xrNode)
        : base(threshold, isReversed)
    {
        _controllerInputDevice = InputDevices.GetDeviceAtXRNode(xrNode);
    }

    public override float GetInputValue() =>
        _controllerInputDevice.TryGetFeatureValue(CommonUsages.grip, out var outvar) ? outvar : 0;
}