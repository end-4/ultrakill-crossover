using PluginConfig.API;
using PluginConfig.API.Fields;
using PluginConfig.API.Decorators;
using System.IO;
using System;
using UnityEngine;

namespace Crossover;

public class ConfigManager {
    public static PluginConfigurator config;

    public static BoolField Enable;
    public static ColorField AccentColor;
    public static FloatField StartScale;
    public static FloatField EndScale;
    public static FloatField EnemyHealthThreshold;
    public static FloatSliderField CrossMarkOpacity;

    public static void Initialize() {
    }

    static ConfigManager() {
        config = PluginConfigurator.Create("Crossover", Plugin.PluginGUID);
        string iconPath = Path.Combine(Plugin.workingDir, "icon.png");
        if (File.Exists(iconPath)) config.SetIconWithURL(iconPath);

        new ConfigHeader(config.rootPanel, "", 10);
        Enable = new BoolField(config.rootPanel, "Enable death crosses", "enable", true);
        AccentColor = new ColorField(config.rootPanel, "Cross accent color", "accentColor", new Color(219f / 255f, 30f / 255f, 57f / 255f));
        StartScale = new FloatField(config.rootPanel, "Cross starting scale", "startScale", 2f);
        EndScale = new FloatField(config.rootPanel, "Cross ending scale", "endScale", 0.35f);
        EnemyHealthThreshold = new FloatField(config.rootPanel, "Enemy health threshold", "enemyHealthThreshold", 0);
        CrossMarkOpacity = new FloatSliderField(config.rootPanel, "Cross mark opacity", "crossMarkOpacity",
            new Tuple<float, float>(0f, 1f), 1f);
    }
}
