using System;
using NukeLib.Utils;
using ThornClient.Core;
using ThornClient.Core.ConfigurableElements;
using ThornClient.Core.DataTypes;
using UnityEngine;

namespace Crossover;

/// <summary>
/// Configuration for Crossover
/// </summary>
public class CrossoverConfig : Module {
    public static CrossoverConfig Instance = null!;

    public override Sprite Icon => FileAssetUtils.LoadNewSprite(Plugin.PluginSymbolicIconPath);

    /// <inheritdoc />
    public override string[] Tags => ["mark", "death", "indicator", "tracker"];

    // Death Crosses
    public static Setting<bool> EnableCrosses;
    public static Setting<Color> AccentColor;

    // Death Crosses - Advanced
    public static Setting<float> StartScale = null!;
    public static Setting<float> EndScale = null!;
    public static Setting<float> EnemyHealthThreshold = null!;
    public static Setting<float> CrossMarkOpacity = null!;
    public static Setting<float> ScalingDelay = null!;
    public static Setting<float> ScalingDuration = null!;
    public static Setting<float> VisibleDuration = null!;

    // Enemy Trackers
    public static Setting<bool> EnableTrackers = null!;
    public static Setting<int> TrackerThreshold = null!;
    public static Setting<EnemyList> ForceTrackEnemies = null!;
    public static Setting<bool> TrackerIgnorePuppets = null!;
    public static Setting<bool> TrackerUseSpecificEnemyIcons = null!;
    public static Setting<bool> TrackerShowEnemyNames = null!;
    public static Setting<Color> TrackerColor = null!;

    // Enemy Trackers - Advanced
    public static Setting<float> TrackerScale = null!;
    public static Setting<float> TrackerMarkOpacity = null!;

    // Internal
    public static Setting<string> LastVersion = null!;

    /// <inheritdoc />
    public CrossoverConfig() : base("crossover.config", "Crossover",
        "Configuration for cross marks and enemy indicators",
        ModuleCategory.Render, hasToggling: false) {
        Instance = this;

        // --- DEATH CROSSES ---
        CreateHeader("deathCrossesHeader", "Death Crosses");
        EnableCrosses = CreateSetting(
            "crossEnabled", "Enable death crosses",
            "Whether to show death cross marks on killed enemies",
            true
        );
        AccentColor = CreateSetting(
            "accentColor", "Cross accent color",
            "The primary accent color for death cross marks",
            0xdb1e39.ToColor()
        );

        // --- DEATH CROSSES : ADVANCED ---
        var crossAdvanced = CreateGroup("crossAdvanced", "Advanced", "Advanced visual options for cross marks");
        StartScale = CreateSetting("crossStartScale", "Cross starting scale", "Initial scale on appearance", 2f);
        EndScale = CreateSetting("crossEndScale", "Cross ending scale", "Final scale before disappearing", 0.35f,
            crossAdvanced);
        EnemyHealthThreshold = CreateSetting("crossEnemyHealthThreshold", "Cross enemy health threshold",
            "At what health amount should the cross trigger", 0f, crossAdvanced);
        EnemyHealthThreshold.Hints = new InterfaceHints { Hidden = true };
        CrossMarkOpacity = CreateSetting(
            "crossMarkOpacity", "Cross mark opacity",
            "Opacity level of the cross mark",
            1f, crossAdvanced
        );
        CrossMarkOpacity.Hints = new InterfaceHints { Range = Tuple.Create(0f, 1f) };
        ScalingDelay = CreateSetting("crossScalingDelay", "Scaling delay",
            "Delay before scale transformation begins",
            0.000f, crossAdvanced
        );
        ScalingDuration = CreateSetting("crossScalingDuration", "Scaling duration",
            "Time duration for scale transition",
            0.1333f, crossAdvanced
        );
        VisibleDuration = CreateSetting("crossVisibleDuration", "Visible duration",
            "Total time the cross remains visible",
            0.5333f, crossAdvanced
        );

        // --- ENEMY TRACKERS ---
        CreateHeader("trackersHeader", "Enemy Trackers");
        EnableTrackers = CreateSetting(
            "trackerEnabled", "Enable tracker icons",
            "Shows icons at enemy locations",
            false
        );
        TrackerThreshold = CreateSetting(
            "trackerThreshold", "Reveal last x enemies",
            "Show trackers for all enemies when there are at most x remaining, where x=",
            5
        );

        ForceTrackEnemies = CreateSetting("trackerForceEnemies", "Force track enemies",
            "These enemies will always be tracked regardless of the global enemy count. Tip: especially useful for Mindflayers and Powers since they teleport a lot and require special attention",
            new EnemyList()
        );
        TrackerShowEnemyNames = CreateSetting(
            "trackerShowEnemyNames", "Show enemy names",
            "Display text labels of enemy names next to their tracker",
            true
        );
        TrackerColor = CreateSetting(
            "trackerColor", "Tracker accent color",
            "The accent color used for enemy indicator marks",
            0xF1B613.ToColor()
        );

        var trackerAdvanced = CreateGroup("crossAdvanced", "Advanced", "Advanced visual options for cross marks");
        TrackerUseSpecificEnemyIcons = CreateSetting(
            "trackerUseSpecificEnemyIcons", "Use specific enemy type icons",
            "Render distinct icons based on enemy type instead of generic icons",
            true, trackerAdvanced
        );
        TrackerIgnorePuppets = CreateSetting(
            "trackerIgnorePuppets", "Ignore puppets (blood bois)",
            "Do not display trackers for violence tree or Deathcatcher-spawned enemies",
            true, trackerAdvanced
        );
        TrackerScale = CreateSetting(
            "trackerScale", "Tracker mark scale",
            "Visual scale multiplier for tracker indicators",
            1f, trackerAdvanced
        );
        TrackerMarkOpacity = CreateSetting(
            "trackerMarkOpacity", "Tracker mark opacity",
            "Opacity level for tracker marks",
            0.7f, trackerAdvanced
        );
        TrackerMarkOpacity.Hints = new InterfaceHints { Range = Tuple.Create(0f, 1f) };

        // --- INTERNAL ---
        var devGroup = CreateGroup("devGroup", "Developer", "Developer options");
        LastVersion = CreateSetting("lastVersion", "Last version", "Internal version tracking", "0.0.0", devGroup);
    }

    /// <summary>
    /// Check if a given enemy type is force-tracked
    /// </summary>
    public static bool IsEnemyForceTracked(EnemyType type) {
        return ForceTrackEnemies.Value.Includes(type);
    }
}
