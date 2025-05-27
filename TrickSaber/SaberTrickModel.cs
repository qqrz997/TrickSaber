using System.Threading.Tasks;
using UnityEngine;

namespace TrickSaber;

public class SaberTrickModel
{
    public Rigidbody Rigidbody { get; private set; } = null!; // set in initializer
    public GameObject GameObject { get; private set; } = null!; // set in initializer
    public Transform Transform { get; private set; } = null!; // set in initializer
    
    public Quaternion DefaultRotation { get; private set; } = Quaternion.identity;
    public Vector3 DefaultPosition { get; private set; } = Vector3.zero;

    public async Task<bool> Init(Saber saber)
    {
        var gameObject = await GetSaberModelFromSaber(saber);
        if (gameObject == null)
        {
            return false;
        }
        GameObject = gameObject;
        Transform = gameObject.transform;
        DefaultPosition = gameObject.transform.localPosition;
        DefaultRotation = gameObject.transform.localRotation;

        Rigidbody = GameObject.AddComponent<Rigidbody>();
        Rigidbody.useGravity = false;
        Rigidbody.isKinematic = true;
        Rigidbody.detectCollisions = false;
        Rigidbody.maxAngularVelocity = 800;
        Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        return true;
    }

    private static async Task<GameObject?> GetSaberModelFromSaber(Saber saber)
    {
        SaberModelController? smc = null;

        var timeout = 2000;
        var interval = 300;
        var time = 0;

        while (!smc)
        {
            smc = saber.GetComponentInChildren<SaberModelController>();

            if (smc) return smc.gameObject;

            if (time > timeout) return null;

            time += interval;
            await Task.Delay(interval);
        }

        return null;
    }
}