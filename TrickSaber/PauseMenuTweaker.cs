using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Tags;
using SiraUtil.Logging;
using TrickSaber.Configuration;
using UnityEngine;
using Zenject;

namespace TrickSaber;

internal class PauseMenuTweaker : IInitializable
{
    private readonly PluginConfig _config;
    private readonly SiraLog _logger;
    private readonly PauseMenuManager _pauseMenuManager;

    public PauseMenuTweaker(
        PluginConfig config,
        SiraLog logger,
        PauseMenuManager pauseMenuManager)
    {
        _config = config;
        _logger = logger;
        _pauseMenuManager = pauseMenuManager;
    }

    public void Initialize()
    {
        try
        {
            var canvas = _pauseMenuManager._levelBar
                .transform
                .parent
                .parent
                .GetComponent<Canvas>();
            if (!canvas) return;

            var toggleObject = new ToggleSettingTag().CreateObject(canvas.transform);
            var toggleTransform = toggleObject.GetComponent<RectTransform>();
            toggleTransform.anchoredPosition = new(26, -15);
            toggleTransform.sizeDelta = new(-130, 7);

            var toggleSetting = toggleObject.GetComponent<ToggleSetting>();
            toggleSetting.TextMesh.text = "TrickSaber Enabled";
            toggleSetting.Value = _config.TrickSaberEnabled;
            toggleSetting.Toggle.onValueChanged.AddListener(enabled => { _config.TrickSaberEnabled = enabled; });
        }
        catch
        {
            _logger.Warn("No checkbox for you sir (Failed to edit pause menu)");
        }
    }
}