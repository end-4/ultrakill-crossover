using HarmonyLib;
using Notiffy.API;
using UnityEngine;

namespace Crossover.Patches;

[HarmonyPatch(typeof(EnemyIdentifier))]
internal class EnemyPatch {
    [HarmonyPostfix]
    [HarmonyPatch("ProcessDeath")]
    static void EnemyDied(EnemyIdentifier __instance) {
        if (!ConfigManager.Enable.value || __instance.health < ConfigManager.EnemyHealthThreshold.value) return;
        GameObject cross = Object.Instantiate(Plugin.DeathCrossPrefab, Plugin.CrossoverCanvas.transform);
        CrossBehavior crossBehavior = cross.AddComponent<CrossBehavior>();
        crossBehavior.SetEnemy(__instance);
        cross.SetActive(true);

        EnemyListener.NotifyChange();
    }

    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    static void EnemySpawned(EnemyIdentifier __instance) {
        if (!ConfigManager.EnableTrackers.value || (__instance.puppet && ConfigManager.TrackerIgnorePuppets.value)) return;
        EnemyIndicatorController controller = __instance.gameObject.AddComponent<EnemyIndicatorController>();
        controller.SetEnemy(__instance);

        EnemyListener.NotifyChange();
    }
}
