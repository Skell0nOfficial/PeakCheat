using HarmonyLib;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(AirportCheckInKiosk), nameof(AirportCheckInKiosk.BeginIslandLoadRPC))]
    internal class FakeLevelPatch
    {
        static bool Prefix(string sceneName, int ascent) => sceneName.Contains("Level_") || sceneName == "WilIsland";
    }
}