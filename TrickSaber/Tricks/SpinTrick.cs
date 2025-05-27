using System;
using System.Collections;
using UnityEngine;

namespace TrickSaber.Tricks;

internal class SpinTrick : Trick
{
    private bool _isVelocityDependent;
    private float _spinSpeed;
    private float _largestSpinSpeed;
    private float _finalSpinSpeed;

    public override TrickAction TrickAction => TrickAction.Spin;

    public override void OnInit()
    {
        _isVelocityDependent = _config.IsSpeedVelocityDependent;
    }

    public override void OnTrickStart()
    {
        _largestSpinSpeed = 0;
        if (_isVelocityDependent)
        {
            var angularVelocity = MovementController.GetAverageAngularVelocity();
            _spinSpeed = Math.Abs(angularVelocity.x) + Math.Abs(angularVelocity.y);
            angularVelocity = Quaternion.Inverse(MovementController.ControllerRotation) * angularVelocity;
            if (angularVelocity.x < 0)
            {
                _spinSpeed *= -1;
            }
        }
        else
        {
            _spinSpeed = _config.SpinDirection == SpinDir.Forward ? 30 : -30;
        }

        _spinSpeed *= _config.SpinSpeed;
    }

    private void Update()
    {
        _finalSpinSpeed = _spinSpeed;
        if (!_isVelocityDependent)
        {
            _finalSpinSpeed *= Mathf.Pow(value, 3f);
        }

        if (Math.Abs(_finalSpinSpeed) > Math.Abs(_largestSpinSpeed))
        {
            _largestSpinSpeed = _finalSpinSpeed;
        }

        SaberTrickModel.Transform.Rotate(Vector3.right * _finalSpinSpeed);
    }

    private IEnumerator LerpToOriginalRotation()
    {
        var rot = SaberTrickModel.Transform.localRotation;
        while (Quaternion.Angle(rot, Quaternion.identity) > 5f)
        {
            rot = Quaternion.Lerp(rot, Quaternion.identity, Time.deltaTime * 20);
            SaberTrickModel.Transform.localRotation = rot;
            yield return new WaitForEndOfFrame();
        }

        OnTrickEndImmediately();
    }

    private IEnumerator CompleteRotation()
    {
        const int minSpeed = 8;
        float largestSpinSpeed = _largestSpinSpeed;

        if (Mathf.Abs(largestSpinSpeed) < minSpeed)
        {
            largestSpinSpeed = largestSpinSpeed < 0 ? -minSpeed : minSpeed;
        }

        float threshold = Mathf.Abs(largestSpinSpeed) + 0.1f;
        float angle = Quaternion.Angle(SaberTrickModel.Transform.localRotation, Quaternion.identity);

        while (angle > threshold)
        {
            SaberTrickModel.Transform.Rotate(Vector3.right * largestSpinSpeed);
            angle = Quaternion.Angle(SaberTrickModel.Transform.localRotation, Quaternion.identity);
            yield return new WaitForEndOfFrame();
        }

        OnTrickEndImmediately();
    }

    public override void OnTrickEndRequested()
    {
        StartCoroutine(_config.CompleteRotationMode ? CompleteRotation() : LerpToOriginalRotation());
    }

    public override void OnTrickEndImmediately()
    {
        SaberTrickModel.Transform.localPosition = SaberTrickModel.DefaultPosition;
        SaberTrickModel.Transform.localRotation = SaberTrickModel.DefaultRotation;
        Reset();
    }
}