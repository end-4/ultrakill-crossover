using HarmonyLib;
using UnityEngine;

namespace Crossover.Patches;

[HarmonyPatch(typeof(EnemyIdentifier))]
public class EnemyPatch {
    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    public static void EnemySpawned(EnemyIdentifier __instance) {
        if (!ConfigManager.Enable.value || __instance.health < ConfigManager.EnemyHealthThreshold.value) return;
        GameObject cross = Object.Instantiate(Plugin.DeathCrossPrefab, Plugin.DeathCrossCanvas.transform);
        cross.GetComponent<CanvasGroup>().alpha = 0;
        CrossBehavior crossBehavior = cross.AddComponent<CrossBehavior>();
        crossBehavior.enemy = __instance;
    }
}
