using HarmonyLib;
using PeakCheat.Classes;
using PeakCheat.Utilities;
using System.Collections.Generic;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(CharacterSyncer), "OnDataReceived")]
    internal class DataPatch
    {
        private static Dictionary<CheatPlayer, CharacterSyncData> _data = new Dictionary<CheatPlayer, CharacterSyncData>();
        public static bool TryGetData(CheatPlayer player, out CharacterSyncData? data)
        {
            if (_data.TryGetValue(player, out var syncData))
            {
                data = syncData;
                return true;
            }

            data = null;
            return false;
        }
        public static void Postfix(CharacterSyncer __instance, CharacterSyncData data)
        {
            if (__instance.TryGetComponent<Character>(out var c))
            {
                _data[c] = data;
                return;
            }

            LogUtil.Log(false, $"[DataPatch] Couldnt get Character Instance for syncer: {__instance.name}");
        }
    }
}