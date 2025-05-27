using System;
using SiraUtil.Logging;
using TrickSaber.Configuration;
using UnityEngine;
using Zenject;

namespace TrickSaber.Tricks;

internal abstract class Trick : MonoBehaviour
{
    protected MovementController MovementController = null!; // set in initializer
    protected SaberTrickManager SaberTrickManager = null!; // set in initializer
    protected SaberTrickModel SaberTrickModel = null!; // set in initializer
    
    public TrickState state = TrickState.Inactive;
    public float value;

    public abstract TrickAction TrickAction { get; }

    public event Action<TrickAction>? TrickStarted;
    public event Action<TrickAction>? TrickEnding;
    public event Action<TrickAction>? TrickEnded;

    [Inject] protected readonly PluginConfig _config = null!;

    public void Init(SaberTrickManager saberTrickManager, MovementController movementController)
    {
        SaberTrickManager = saberTrickManager;
        MovementController = movementController;
        SaberTrickModel = SaberTrickManager.SaberTrickModel;
        OnInit();
        Plugin.Log.Debug($"Trick: {TrickAction} initialized");
    }

    private void Awake()
    {
        enabled = false;
    }

    public bool StartTrick()
    {
        if (state != TrickState.Inactive) return false;

        enabled = true;
        state = TrickState.Started;
        OnTrickStart();
        TrickStarted?.Invoke(TrickAction);
        return true;
    }

    public void EndTrick()
    {
        if (state == TrickState.Started)
        {
            enabled = false;
            state = TrickState.Ending;
            TrickEnding?.Invoke(TrickAction);
            OnTrickEndRequested();
        }
    }

    protected void Reset()
    {
        state = TrickState.Inactive;
        TrickEnded?.Invoke(TrickAction);
    }

    public abstract void OnTrickStart();

    public abstract void OnTrickEndRequested();

    public abstract void OnTrickEndImmediately();

    public abstract void OnInit();

    internal class Factory : PlaceholderFactory<Type, GameObject, Trick> { }

    internal class CustomFactory : IFactory<Type, GameObject, Trick>
    {
        private readonly DiContainer _container;

        private CustomFactory(DiContainer container)
        {
            _container = container;
        }

        public Trick Create(Type trickType, GameObject gameObject)
        {
            return (Trick) _container.InstantiateComponent(trickType, gameObject);
        }
    }
}