using HarmonyLib;
using PeakCheat.Utilities;
using Steamworks;

namespace PeakCheat.Patches
{
    [HarmonyPatch]
    public class SteamPatches
    {
        #if DEBUG
        [HarmonyPatch(typeof(SteamAPI), "Init")]
        [HarmonyPostfix]
        public static void UpdateSteamID(ref bool __result)
        {
            if (__result)
            {
                var app = SteamUtils.GetAppID().m_AppId;
                var account = SteamUser.GetSteamID().m_SteamID;

                LogUtil.Log($"[{app}] Logged Into {account}");
            }
        }
        #endif
    }
}
