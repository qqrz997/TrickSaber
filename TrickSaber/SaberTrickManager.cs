using System.Collections.Generic;
using SiraUtil.Logging;
using TrickSaber.Configuration;
using TrickSaber.InputHandling;
using TrickSaber.Tricks;
using UnityEngine;
using Zenject;

namespace TrickSaber;

internal class SaberTrickManager : MonoBehaviour
{
    private readonly Dictionary<TrickAction, Trick> _tricks = new Dictionary<TrickAction, Trick>();

    public SaberTrickModel SaberTrickModel = null!;

    private VRController _vrController = null!;

    private Saber _saber = null!;

    private PluginConfig _config = null!;
    private GlobalTrickManager _globalTrickManager = null!;
    private SiraLog _logger = null!;
    private PauseController _pauseController = null!;
    private MovementController _movementController = null!;
    private InputManager _inputManager = null!;
    private AudioTimeSyncController _audioTimeSyncController = null!;

    private Trick.Factory _trickFactory = null!;

    [Inject]
    private void Construct(
        PluginConfig config,
        SiraLog logger,
        [InjectOptional] PauseController pauseController,
        MovementController movementController,
        InputManager inputManager,
        SaberControllerBearer saberControllerBearer,
        SaberType saberType,
        SaberTrickModel saberTrickModel,
        AudioTimeSyncController audioTimeSyncController,
        Trick.Factory trickFactory)
    {
        _config = config;
        _logger = logger;
        _pauseController = pauseController;
        _movementController = movementController;
        _inputManager = inputManager;
        _audioTimeSyncController = audioTimeSyncController;
        SaberTrickModel = saberTrickModel;

        _saber = saberControllerBearer[saberType].Saber;
        _vrController = saberControllerBearer[saberType].VRController;

        _trickFactory = trickFactory;
    }

    public async void Init(GlobalTrickManager globalTrickManager)
    {
        _globalTrickManager = globalTrickManager;

        _logger.Debug($"Instantiated on {gameObject.name}");

        if (!_vrController)
        {
            _logger.Error("Controller not present");
            Cleanup();
            return;
        }

        if (_saber.saberType == SaberType.SaberA)
        {
            _globalTrickManager.LeftSaberTrickManager = this;
        }
        else
        {
            _globalTrickManager.RightSaberTrickManager = this;
        }

        _movementController.Init(_vrController);

        _inputManager.Init(_saber.saberType);
        _inputManager.TrickActivated += OnTrickActivated;
        _inputManager.TrickDeactivated += OnTrickDeactivated;

        if (!await SaberTrickModel.Init(_saber))
        {
            _logger.Error("Couldn't get saber model");
            Cleanup();
            return;
        }
        
        _movementController.enabled = true;

        AddTrick<SpinTrick>();
        AddTrick<ThrowTrick>();

        if (_pauseController)
        {
            _pauseController.didResumeEvent += EndAllTricks;
        }

        _logger.Info($"Trick Manager initialized {_tricks.Count} {(_tricks.Count == 1 ? "trick" : "tricks")}.");
    }

    private void Cleanup()
    {
        foreach (var trick in _tricks.Values)
        {
            DestroyImmediate(trick);
        }

        DestroyImmediate(_movementController);
        DestroyImmediate(this);
    }

    private void Update()
    {
        _inputManager.Tick();
    }

    private void OnTrickDeactivated(TrickAction trickAction)
    {
        if (_tricks.TryGetValue(trickAction, out var trick) && trick.state == TrickState.Started)
        {
            trick.EndTrick();
        }
    }

    private void OnTrickActivated(TrickAction trickAction, float val)
    {
        if (!CanDoTrick() || !_tricks.TryGetValue(trickAction, out var trick))
        {
            return;
        }

        trick.value = val;

        if (trick.state == TrickState.Inactive
            && _audioTimeSyncController.state != AudioTimeSyncController.State.Paused)
        {
            trick.StartTrick();
        }
    }

    private void AddTrick<T>() where T : Trick
    {
        var trick = _trickFactory.Create(typeof(T), gameObject);
        trick.Init(this, _movementController);
        trick.TrickStarted += _globalTrickManager.OnTrickStarted;
        trick.TrickEnding += _globalTrickManager.OnTrickEnding;
        trick.TrickEnded += _globalTrickManager.OnTrickEnded;
        _tricks.Add(trick.TrickAction, trick);
    }

    public bool IsTrickInState(TrickAction trickAction, TrickState state)
    {
        return _tricks.TryGetValue(trickAction, out var trick) && trick.state == state;
    }

    public bool IsDoingTrick()
    {
        foreach (var trick in _tricks.Values)
        {
            if (trick.state != TrickState.Inactive) return true;
        }

        return false;
    }

    public void EndAllTricks()
    {
        foreach (var trick in _tricks.Values)
        {
            trick.OnTrickEndImmediately();
        }
    }

    private bool CanDoTrick()
    {
        return _config.TrickSaberEnabled;
        //&& _globalTrickManager.CanDoTrick();
    }
}