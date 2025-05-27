using UnityEngine;
using UnityEngine.XR;

namespace TrickSaber.InputHandling;

internal class ThumbstickHandler : InputHandler
{
    private readonly string _inputString;

    public ThumbstickHandler(
        float threshold,
        bool isReversed,
        ThumstickDir thumstickDir,
        XRNode node) : base(threshold, isReversed)
    {
        _inputString = $"{thumstickDir}{(node == XRNode.LeftHand ? "LeftHand" : "RightHand")}";
    }

    public override float GetInputValue()
    {
        return Input.GetAxis(_inputString);
    }
}