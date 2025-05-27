using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using SiraUtil.Submissions;
using TrickSaber.Configuration;
using Zenject;

namespace TrickSaber;

internal class ScoreSubmissionManager : IInitializable
{
    private readonly PluginConfig _config;
    private readonly Submission _submission;

    public ScoreSubmissionManager(PluginConfig config, Submission submission)
    {
        _config = config;
        _submission = submission;
    }
    
    public void Initialize()
    {
        var settings = typeof(PluginConfig)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(prop => prop.PropertyType == typeof(bool));

        foreach (var setting in settings)
        {
            if (TryGetDisableReason(setting, out var reason))
            {
                _submission.DisableScoreSubmission(nameof(TrickSaber), reason);
                break;
            }
        }
    }

    private static bool TryGetDisableReason(PropertyInfo property, [NotNullWhen(true)] out string? reason)
    {
        if (Attribute.GetCustomAttribute(
                property,
                typeof(DisablesScoringAttribute))
            is DisablesScoringAttribute disablesScoringAttribute)
        {
            reason = disablesScoringAttribute.Reason ?? property.Name;
            return true;
        }
        reason = null;
        return false;
    }
}