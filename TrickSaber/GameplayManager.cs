using System;
using System.Reflection;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using SiraUtil.Logging;
using SiraUtil.Submissions;
using TrickSaber.Configuration;
using UnityEngine;
using Zenject;

namespace TrickSaber;

internal class GameplayManager : IInitializable
{
    private readonly PluginConfig _config;
    private readonly SiraLog _logger;
    private readonly Submission _submission;
    private readonly PauseMenuManager _pauseMenuManager;

    public GameplayManager(PluginConfig config, SiraLog logger, Submission submission, [InjectOptional] PauseMenuManager pauseMenuManager)
    {
        _config = config;
        _logger = logger;
        _submission = submission;
        _pauseMenuManager = pauseMenuManager;
    }

    public void DisableScoreSubmissionIfNeeded()
    {
        foreach (var propertyInfo in typeof(PluginConfig).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (propertyInfo.PropertyType!=typeof(bool)) continue;

            if (Attribute.GetCustomAttribute(propertyInfo, typeof(DisablesScoringAttribute)) 
                is DisablesScoringAttribute attr)
            {
                DisableScore(
                    (bool)propertyInfo.GetValue(_config),
                    attr.Reason is null or [] ? propertyInfo.Name : attr.Reason);
            }
        }
    }

    public void DisableScore(bool disable, string reason)
    {
        if (!disable) return;
        _submission.DisableScoreSubmission("Tricksaber", reason);
    }

    public void Initialize()
    {
        DisableScoreSubmissionIfNeeded();

        try
        {
            CreateCheckbox();
        }
        catch
        {
            _logger.Warn($"No checkbox for you sir");
        }
    }

    public void CreateCheckbox()
    {
        if (_pauseMenuManager == null) return;

        var canvas = _pauseMenuManager._levelBar
            .transform
            .parent
            .parent
            .GetComponent<Canvas>();
        if (!canvas) return;

        var toggleObject = new ToggleSettingTag().CreateObject(canvas.transform);

        ((RectTransform)toggleObject.transform).anchoredPosition = new Vector2(26, -15);
        ((RectTransform)toggleObject.transform).sizeDelta = new Vector2(-130, 7);

        toggleObject.transform.Find("NameText").GetComponent<CurvedTextMeshPro>().text = "Tricksaber Enabled";

        var toggleSetting = toggleObject.GetComponent<ToggleSetting>();
        toggleSetting.Value = _config.TrickSaberEnabled;
        toggleSetting.Toggle.onValueChanged.AddListener(enabled => { _config.TrickSaberEnabled = enabled; });
    }
}