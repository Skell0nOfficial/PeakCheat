using HarmonyLib;
using PeakCheat.Main;
using UnityEngine;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(CharacterMovement), "Update")]
    internal class MovementPatch
    {
        private static bool _forceFreeze = false;
        public static void Freeze(bool enable) => _forceFreeze |= enable;
        public static bool Prefix()
        {
            bool StopMovement = !CanMove();
            if (StopMovement) Character.localCharacter.Invoke("ResetInput", float.Epsilon);
            return !StopMovement;
        }
        private static bool CanMove()
        {
            if (UIHandler.GUIActive() || _forceFreeze) return false;

            return true;
        }
    }
}