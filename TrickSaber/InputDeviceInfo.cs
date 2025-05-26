using UnityEngine.XR;
using Zenject;

namespace TrickSaber;

internal class InputDeviceInfo : IInitializable
{
    public string Name { get; private set; } = "Unknown";
    
    public void Initialize()
    {
        var inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (!inputDevice.isValid) inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (inputDevice.isValid)
        {
            Name = inputDevice.name;
        }
    }
}