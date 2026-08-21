using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Crossover;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency("com.github.end-4.thornClient")]
[BepInDependency("com.github.end-4.notiffy")]
[BepInDependency("com.github.end-4.nukeLib")]
public class Plugin : BaseUnityPlugin {
    // Logger
    internal static ManualLogSource? Log;

    // Plugin config
    public static string workingPath = Assembly.GetExecutingAssembly().Location;
    public static string workingDir = Path.GetDirectoryName(workingPath);
    public const string PluginGUID = "com.github.end-4.crossover";
    public const string PluginName = "Crossover";
    public const string PluginVersion = "2.0.0";
    public static string PluginIconPath => Path.Combine(workingDir, "icon.png");
    public static string PluginSymbolicIconPath => Path.Combine(workingDir, "icon_clickgui.png");

    internal static GameObject DeathCrossPrefab;
    internal static GameObject EnemyIndicatorPrefab;
    internal static GameObject CrossoverCanvasPrefab;
    internal static GameObject CrossoverCanvas;
    internal static Dictionary<string, Sprite> EnemyIcons = new();
    private static readonly string BundlePath = Path.Combine(workingDir, "assets", "crossover.bundle");

    internal static string[] IconNames = {
        "dot_circle",
        "big_johninator",
        "cancerous_rodent",
        "centaur_mortar",
        "centaur_orb",
        "centaur_rocket",
        "cerberus",
        "deathcatcher",
        "drone",
        "ferryman",
        "filth",
        "flesh_panopticon",
        "flesh_prison",
        "gabriel",
        "gabriel_second",
        "gutterman",
        "guttertank",
        "hideous_mass",
        "idol",
        "malicious_face",
        "mandalore",
        "mindflayer",
        "minos_prime",
        "minotaur",
        "mirror_reaper",
        "power",
        "providence",
        "puppet",
        "schism",
        "sisyphus",
        "sisyphus_prime",
        "soldier",
        "stalker",
        "stray",
        "mannequin",
        "streetcleaner",
        "swordsmachine",
        "turret",
        "v2",
        "very_cancerous_rodent",
        "virtue"
    };

    private void LoadObjects() {
        AssetBundle bundle = AssetBundle.LoadFromFile(BundlePath);
        if (bundle == null) {
            Log.LogError("Couldn't load asset bundle");
        }

        DeathCrossPrefab = bundle.LoadAsset<GameObject>("DeathCross");
        EnemyIndicatorPrefab = bundle.LoadAsset<GameObject>("EnemyIndicator");
        CrossoverCanvasPrefab = bundle.LoadAsset<GameObject>("CrossoverCanvas");
        CrossoverCanvas = Instantiate(CrossoverCanvasPrefab);
        DontDestroyOnLoad(CrossoverCanvas);
        CrossoverCanvas.hideFlags = HideFlags.HideAndDontSave; // Idk if this is bad but EladNLG's Healthbars does it
        // Load icons
        for (int i = 0; i < IconNames.Length; i++) {
            string iconName = IconNames[i];
            Sprite iconSprite = bundle.LoadAsset<Sprite>(iconName);
            EnemyIcons.Add(iconName, iconSprite);
        }
    }

    private void Awake() {
        Log = Logger;

        // Load stuff
        // CrossoverConfig initialized by Thorn
        LoadObjects();

        // Patch stuff
        Harmony harmony = new Harmony("Crossover");
        harmony.PatchAll();

        // Done
        UserHints.Initialize();
        Log.LogInfo("Crossover loaded!");
    }
}
