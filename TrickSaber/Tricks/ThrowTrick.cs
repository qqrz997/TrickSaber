using System.Collections;
using UnityEngine;

namespace TrickSaber.Tricks;

internal class ThrowTrick : Trick
{
    private float _controllerSnapThreshold = 0.3f;
    private float _saberRotSpeed;
    private float _velocityMultiplier = 1;
    private Transform? _originalParent;

    public override TrickAction TrickAction => TrickAction.Throw;
    
    public override void OnInit()
    {
        _controllerSnapThreshold = _config.ControllerSnapThreshold;
        _velocityMultiplier = _config.ThrowVelocity;
    }
    
    public override void OnTrickStart()
    {
        _originalParent = SaberTrickModel.Transform.parent;
        SaberTrickModel.Transform.SetParent(null);
        
        SaberTrickModel.Rigidbody.isKinematic = false;
        Vector3 finalVelocity = MovementController.GetAverageVelocity() * _velocityMultiplier;
        SaberTrickModel.Rigidbody.velocity = finalVelocity * 3;
        _saberRotSpeed = finalVelocity.magnitude;
        _saberRotSpeed *= Mathf.Sign(MovementController.AngularVelocity.x) * 150f;
        SaberTrickModel.Rigidbody.AddRelativeTorque(Vector3.right * _saberRotSpeed, ForceMode.Acceleration);
    }

    public override void OnTrickEndRequested()
    {
        if (!SaberTrickModel.Rigidbody) return;
        SaberTrickModel.Rigidbody.velocity = Vector3.zero;
        StartCoroutine(ReturnSaber(_config.ReturnSpeed));
    }

    public override void OnTrickEndImmediately()
    {
        StopAllCoroutines();
        ThrowEnd();
    }

    public IEnumerator ReturnSaber(float speed)
    {
        SaberTrickModel.Rigidbody.AddRelativeTorque(Vector3.right * speed * (_saberRotSpeed<0?-1:1) * _config.ReturnSpinMultiplier, ForceMode.VelocityChange);
        Vector3 position = SaberTrickModel.Transform.position;
        var controllerPos = MovementController.ControllerPosition;
        float distance = Vector3.Distance(position, controllerPos);
        while (distance > _controllerSnapThreshold)
        {
            distance = Vector3.Distance(position, controllerPos);
            var direction = controllerPos - position;
            float force;
            if (distance < 1f) force = 10f;
            else force = speed * distance;
            force = Mathf.Clamp(force, 0, 200);
            SaberTrickModel.Rigidbody.velocity = direction.normalized * force;
            position = SaberTrickModel.GameObject.transform.position;
            controllerPos = MovementController.ControllerPosition;

            yield return new WaitForEndOfFrame();
        }

        ThrowEnd();
    }

    private void ThrowEnd()
    {
        SaberTrickModel.Rigidbody.isKinematic = true;
        SaberTrickModel.Transform.SetParent(_originalParent);
        SaberTrickModel.Transform.localPosition = SaberTrickModel.DefaultPosition;
        SaberTrickModel.Transform.localRotation = SaberTrickModel.DefaultRotation;
        Reset();
    }
}