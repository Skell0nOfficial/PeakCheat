using HarmonyLib;
using UnityEngine;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(CharacterMovement), "GetGravityForce")]
    internal class GravityPatch
    {
        private static bool _gravity = true;
        public static void Gravity(bool enable) => _gravity = enable;
        private static void Postfix(ref Vector3 __result)
        {
            if (!_gravity) __result = Vector3.zero;
        }
    }
}