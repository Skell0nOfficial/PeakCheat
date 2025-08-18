using HarmonyLib;
using Steamworks;
using System.Collections.Generic;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(SteamLobbyHandler), "OnLobbyChat")]
    public class BanPatch
    {
        private static List<ulong> _bannedIDs = new List<ulong>();
        public static bool IsBanned(ulong steamID) => _bannedIDs.Contains(steamID);
        static bool Prefix(SteamLobbyHandler __instance, LobbyChatMsg_t param)
        {
            var lobby = Traverse.Create(__instance)?.Field("m_currentLobby")?.GetValue<CSteamID>()?? CSteamID.Nil;
            if (lobby == CSteamID.Nil) return false;
            if (param.m_ulSteamIDLobby != lobby.m_SteamID) return false;

            return !IsBanned(param.m_ulSteamIDUser);
        }
    }
}