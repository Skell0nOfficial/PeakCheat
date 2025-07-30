using HarmonyLib;

namespace PeakCheat.Utilities
{
    [HarmonyPatch(typeof(CharacterSyncer), nameof(CharacterSyncer.GetDataToWrite))]
    internal class DataSpoofer
    {
        private static void Postfix(ref CharacterSyncData __result)
        {
            
        }
    }
}
