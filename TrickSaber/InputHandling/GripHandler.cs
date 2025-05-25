using UnityEngine.XR;

namespace TrickSaber.InputHandling;

internal class GripHandler : InputHandler
{
    private InputDevice _controllerInputDevice;

    public GripHandler(InputDevice controllerInputDevice, float threshold, bool isReversed = false)
        : base(threshold, isReversed) => 
        _controllerInputDevice = controllerInputDevice;

    public override float GetInputValue() =>
        _controllerInputDevice.TryGetFeatureValue(CommonUsages.grip, out var outvar) ? outvar : 0;
}