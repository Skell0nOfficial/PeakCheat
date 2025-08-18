using HarmonyLib;
using System;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(CloudAPI), "CheckVersion")]
    class Patch
    {
        public static bool Prefix(Action<LoginResponse> response)
        {
            return false;
        }
    }
}