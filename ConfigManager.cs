using PluginConfig.API;
using PluginConfig.API.Fields;
using PluginConfig.API.Decorators;
using System.IO;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Crossover;

public class ConfigManager {
    public static PluginConfigurator config;

    public static BoolField Enable;
    public static ColorField AccentColor;
    public static BoolField EnableTrackers;
    public static IntField TrackerThreshold;
    public static Dictionary<string, BoolField> ForceTrackEnemies = new Dictionary<string, BoolField>();
    public static BoolField TrackerIgnorePuppets;
    public static BoolField TrackerUseSpecificEnemyIcons;
    public static BoolField TrackerShowEnemyNames;
    public static ColorField TrackerColor;

    public static FloatField StartScale;
    public static FloatField EndScale;
    public static FloatField EnemyHealthThreshold;
    public static FloatSliderField CrossMarkOpacity;
    public static FloatField ScalingDuration;
    public static FloatField ScalingDelay;
    public static FloatField VisibleDuration;

    public static FloatField TrackerScale;
    public static FloatSliderField TrackerMarkOpacity;

    public static StringField LastVersion;

    public static void Initialize() {
    }

    public static bool IsEnemyForceTracked(EnemyType type) {
        return ForceTrackEnemies[type.ToString()].value;
    }

    private static void CreateTrackerForcedEnemies() {
        ConfigPanel forceTrackerPanel =
            new ConfigPanel(config.rootPanel, "Always track enemy types...", "forcedTrackEnemies");
        new ConfigHeader(config.rootPanel, "", 10);
        new ConfigHeader(forceTrackerPanel,
            "You can, for example, force Powers and Mindflayers to always have an indicator shown. FUCK POWERS FUCK POWERS FUCK POWERS I HATE POWERS",
            12, TextAlignmentOptions.Left);
        string[] names = Enum.GetNames(typeof(EnemyType));
        Array.Sort(names);
        for (int i = 0; i < names.Length; i++) {
            string name = names[i];
            ForceTrackEnemies[name] = new BoolField(forceTrackerPanel, name, $"forceTrack{name}", false);
        }
    }

    static ConfigManager() {
        config = PluginConfigurator.Create("Crossover", Plugin.PluginGUID);
        string iconPath = Path.Combine(Plugin.workingDir, "icon.png");
        if (File.Exists(iconPath)) config.SetIconWithURL(iconPath);

        new ConfigHeader(config.rootPanel, "", 10);
        new ConfigHeader(config.rootPanel, "-- <color=#db1e39>DEATH CROSSES</color> --", 22);
        Enable = new BoolField(config.rootPanel, "Enable death crosses", "enable", true);
        AccentColor = new ColorField(config.rootPanel, "Cross accent color", "accentColor",
            new Color(219f / 255f, 30f / 255f, 57f / 255f));

        new ConfigHeader(config.rootPanel, "", 10);
        new ConfigHeader(config.rootPanel, "-- <color=#f1b613>ENEMY TRACKERS</color> --", 22);
        new ConfigHeader(config.rootPanel, "Shows pointers for enemy locations even when they're out of view", 12);
        EnableTrackers = new BoolField(config.rootPanel, "Enable tracker icons", "enableTrackers", false);
        TrackerThreshold = new IntField(config.rootPanel, "Reveal last remaining x enemies", "trackerThreshold", 5);
        CreateTrackerForcedEnemies();
        TrackerIgnorePuppets =
            new BoolField(config.rootPanel, "Ignore puppets (blood bois)", "trackerIgnorePuppets", false);
        TrackerUseSpecificEnemyIcons = new BoolField(config.rootPanel, "Use specific enemy type icons",
            "trackerUseSpecificEnemyIcons", true);
        TrackerShowEnemyNames = new BoolField(config.rootPanel, "Show enemy names", "trackerShowEnemyNames", true);
        TrackerColor = new ColorField(config.rootPanel, "Tracker accent color", "trackerColor",
            new Color(241f / 255f, 182f / 255f, 19f / 255f));

        new ConfigHeader(config.rootPanel, "", 10);
        new ConfigHeader(config.rootPanel, "-- <color=#ff7f27>ADVANCED</color> --", 22);
        new ConfigHeader(config.rootPanel, "// <color=#db1e39>Death crosses</color>", 16, TextAlignmentOptions.Left);
        StartScale = new FloatField(config.rootPanel, "Cross starting scale", "startScale", 2f);
        EndScale = new FloatField(config.rootPanel, "Cross ending scale", "endScale", 0.35f);
        EnemyHealthThreshold =
            new FloatField(config.rootPanel, "Cross enemy health threshold", "enemyHealthThreshold", 0);
        CrossMarkOpacity = new FloatSliderField(config.rootPanel, "Cross mark opacity", "crossMarkOpacity",
            new Tuple<float, float>(0f, 1f), 1f);
        ScalingDelay = new FloatField(config.rootPanel, "Scaling delay", "scalingDelay", 0.000f);
        ScalingDuration = new FloatField(config.rootPanel, "Scaling duration", "scalingDuration", 0.1333f);
        VisibleDuration = new FloatField(config.rootPanel, "Visible duration", "visibleDuration", 0.5333f);
        new ConfigHeader(config.rootPanel, "// <color=#f1b613>Enemy trackers</color>", 16, TextAlignmentOptions.Left);
        TrackerScale = new FloatField(config.rootPanel, "Tracker mark scale", "trackerScale", 1f);
        TrackerMarkOpacity = new FloatSliderField(config.rootPanel, "Tracker mark opacity", "trackerMarkOpacity",
            new Tuple<float, float>(0f, 1f), 0.7f);

        // Internal
        LastVersion = new StringField(config.rootPanel, "Last version", "lastVersion", "0.0.0");
        LastVersion.hidden = true;
    }
}
