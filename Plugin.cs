using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Crossover;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency("com.eternalUnion.pluginConfigurator")]
public class Plugin : BaseUnityPlugin {
    // Logger
    internal static ManualLogSource? Log;

    // Plugin config
    public static string workingPath = Assembly.GetExecutingAssembly().Location;
    public static string workingDir = Path.GetDirectoryName(workingPath);
    public const string PluginGUID = "com.github.end-4.crossover";
    public const string PluginName = "Crossover";
    public const string PluginVersion = "1.1.0";

    internal static GameObject DeathCrossPrefab;
    internal static GameObject EnemyIndicatorPrefab;
    internal static GameObject CrossoverCanvasPrefab;
    internal static GameObject CrossoverCanvas;
    private static readonly string BundlePath = Path.Combine(workingDir, "assets", "crossover.bundle");

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
    }

    private void Awake() {
        Log = Logger;

        // Load stuff
        ConfigManager.Initialize();
        LoadObjects();

        // Patch stuff
        Harmony harmony = new Harmony("Crossover");
        harmony.PatchAll();

        // Done
        UserHints.Initialize();
        Log.LogInfo("Crossover loaded!");
    }
}
