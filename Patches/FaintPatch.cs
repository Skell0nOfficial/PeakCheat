using HarmonyLib;
using System;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(Character), "RPCA_PassOut")]
    internal class FaintPatch
    {
        public static Action<Character> PassOut = new Action<Character>(C => {});
        public static void Postfix(Character __instance) => PassOut(__instance);
    }
}