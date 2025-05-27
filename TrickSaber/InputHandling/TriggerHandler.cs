using UnityEngine;
using UnityEngine.XR;

namespace TrickSaber.InputHandling;

internal class TriggerHandler : InputHandler
{
    private readonly string _inputString;

    public TriggerHandler(
        float threshold,
        bool isReversed,
        XRNode node) : base(threshold, isReversed)
    {
        _inputString = node == XRNode.LeftHand ? "TriggerLeftHand" : "TriggerRightHand";
    }

    public override float GetInputValue()
    {
        return Input.GetAxis(_inputString);
    }
}