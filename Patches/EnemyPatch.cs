using HarmonyLib;
using UnityEngine;

namespace Crossover.Patches;

[HarmonyPatch(typeof(EnemyIdentifier))]
class EnemyPatch {
    [HarmonyPostfix]
    [HarmonyPatch("ProcessDeath")]
    static void Postfix(EnemyIdentifier __instance) {
        GameObject cross = Object.Instantiate(Plugin.DeathCrossPrefab, Plugin.DeathCrossCanvas.transform);
        CrossBehavior crossBehavior = cross.AddComponent<CrossBehavior>();
        crossBehavior.enemy = __instance;
        cross.SetActive(true);
    }
}
