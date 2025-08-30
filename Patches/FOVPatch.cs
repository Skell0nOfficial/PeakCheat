using HarmonyLib;
using PeakCheat.Utilities;
using UnityEngine;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(MainCameraMovement), "GetFov")]
    internal class FOVPatch
    {
        private const string timeKey = "FOVPatch::TimeDelay";
        private static bool patchEnabled => !TimeUtil.CheckTime(timeKey);
        private static float forcedFOV = 90f;
        public static float CurrentFOV
        {
            get => patchEnabled? forcedFOV: Camera.main.fieldOfView;
            set
            {
                forcedFOV = value;
                TimeUtil.SetTime(timeKey, .1f);
            }
        }
        static void Postfix(ref float __result) => __result = patchEnabled? forcedFOV: __result;
    }
}
