using HarmonyLib;
using UnityEngine;

namespace Crossover.Patches;

[HarmonyPatch(typeof(EnemyIdentifier))]
internal class EnemyPatch {
    [HarmonyPostfix]
    [HarmonyPatch("ProcessDeath")]
    static void EnemyDied(EnemyIdentifier __instance) {
        if (!CrossoverConfig.EnableCrosses.Value || __instance.health < CrossoverConfig.EnemyHealthThreshold.Value) return;
        GameObject cross = Object.Instantiate(Plugin.DeathCrossPrefab, Plugin.CrossoverCanvas.transform);
        CrossBehavior crossBehavior = cross.AddComponent<CrossBehavior>();
        crossBehavior.SetEnemy(__instance);
        cross.SetActive(true);

        EnemyListener.NotifyChange();
    }

    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    static void EnemySpawned(EnemyIdentifier __instance) {
        // int instanceId = __instance.GetInstanceID();
        // Plugin.Log.LogInfo($"EnemyIdentifier Start instance {instanceId}");
        if (!CrossoverConfig.EnableTrackers.Value || (__instance.puppet && CrossoverConfig.TrackerIgnorePuppets.Value)) return;
        EnemyIndicatorController controller = __instance.gameObject.GetComponent<EnemyIndicatorController>();
        if (controller != null) return;
        controller = __instance.gameObject.AddComponent<EnemyIndicatorController>();
        // Plugin.Log.LogInfo($"Set enemy for {instanceId}, enemy: {__instance.FullName}");
        controller.SetEnemy(__instance);

        EnemyListener.NotifyChange();
    }
}
