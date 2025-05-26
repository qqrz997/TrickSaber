using System.Threading.Tasks;
using SiraUtil.Sabers;
using TrickSaber.Configuration;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace TrickSaber;

public class SaberTrickModel
{
    public Rigidbody? Rigidbody { get; private set; }
    public GameObject? OriginalSaberModel { get; private set; }
    public GameObject? TrickModel { get; private set; }

    private readonly PluginConfig _config;
    private readonly SiraSaberFactory _saberFactory;
    private SiraSaber? _siraSaber;
    private Transform? _saberTransform;

    private readonly bool _isMultiplayer;

    private SaberTrickModel(PluginConfig config, SiraSaberFactory saberFactory, [InjectOptional] MultiplayerPlayersManager multiplayerPlayersManager)
    {
        _config = config;
        _saberFactory = saberFactory;

        _isMultiplayer = multiplayerPlayersManager != null;
    }

    public async Task<bool> Init(Saber saber)
    {
        _siraSaber = _saberFactory.Spawn(saber.saberType);

        OriginalSaberModel = await GetSaberModel(saber);

        if (!OriginalSaberModel)
        {
            Object.DestroyImmediate(_siraSaber.gameObject);
            return false;
        }

        TrickModel = _siraSaber.gameObject;
        _saberTransform = _siraSaber.transform;

        TrickModel.name = $"TrickModel {saber.saberType}";
        AddRigidbody(TrickModel);

        TrickModel.SetActive(false);

        if (!_config.HitNotesDuringTrick || _isMultiplayer)
        {
            _siraSaber.Saber.enabled = false;
        }

        return true;
    }

    public void AddRigidbody(GameObject saberObject)
    {
        Rigidbody = saberObject.AddComponent<Rigidbody>();
        Rigidbody.useGravity = false;
        Rigidbody.isKinematic = true;
        Rigidbody.detectCollisions = false;
        Rigidbody.maxAngularVelocity = 800;
        Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void ChangeToTrickModel()
    {
        if (TrickModel != null)
        {
            TrickModel.SetActive(true);
        }

        if (_saberTransform != null && OriginalSaberModel != null)
        {
            _saberTransform.position = OriginalSaberModel.transform.position;
            _saberTransform.rotation = OriginalSaberModel.transform.rotation;
            OriginalSaberModel.SetActive(false);
        }
    }

    public void ChangeToActualSaber()
    {
        if (OriginalSaberModel != null) OriginalSaberModel.SetActive(true);
        if (TrickModel != null) TrickModel.SetActive(false);
    }

    private async Task<GameObject?> GetSaberModel(Saber saber)
    {
        //return saber.GetComponentInChildren<SaberModelController>()?.gameObject;
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