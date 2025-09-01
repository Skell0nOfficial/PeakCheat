using HarmonyLib;
using PeakCheat.Utilities;
using UnityEngine;

namespace PeakCheat.Patches
{
    [HarmonyPatch]
    internal class BlackScreenPatch
    {
        [HarmonyPatch(typeof(Character), nameof(Character.WarpPlayerRPC))]
        [HarmonyPrefix]
        static bool TP(Character __instance, Vector3 position, bool poof) => (!__instance.IsLocal || Vector3.Distance(__instance.Center, position) <= 500f) && position.All(F => F.IsValid(float.MaxValue / 250f));
        [HarmonyPatch(typeof(CharacterSyncer), nameof(CharacterSyncer.InterpolateRigPositions))]
        [HarmonyPrefix]
        static bool Move(CharacterSyncer __instance)
        {
            if (!__instance.RemoteValue.IsSome) return false;

            return Vector3.Distance(__instance.RemoteValue.Value.hipLocation, PlayerUtil.GetRespawnPosition()) <= 100000F;
        }
    }
}