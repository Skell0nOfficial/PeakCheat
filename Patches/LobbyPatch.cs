using HarmonyLib;
using PeakCheat.Types;
using PeakCheat.Utilities;
using Steamworks;
using Zorro.Core;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(SteamLobbyHandler))]
    internal class LobbyPatch : CheatBehaviour
    {
        bool CheatBehaviour.DelayStart() => true;
        void CheatBehaviour.Start()
        {
            PhotonCallbacks.LeftRoom += () => _currentLobby = CSteamID.Nil;
            PhotonCallbacks.JoinRoomFailed += (C, M) => _currentLobby = CSteamID.Nil;
        }
        static CSteamID _currentLobby = CSteamID.Nil;
        static string? _lobbyLink = null;
        static bool _block = false;
        public static bool GetCurrentLobby(out CSteamID Lobby)
        {
            Lobby = _currentLobby;

            if (Lobby != CSteamID.Nil)
            {
                _block = true;

                if (!SteamMatchmaking.RequestLobbyData(Lobby)) return false;

                int count = SteamMatchmaking.GetNumLobbyMembers(Lobby);
                int limit = SteamMatchmaking.GetLobbyMemberLimit(Lobby);

                LogUtil.Log($"Lobby ID: {Lobby} | Players: {count}/{limit} {(string.IsNullOrEmpty(_lobbyLink) ? "" : $"| Lobby Link: {_lobbyLink}")}");

                return count > 0 && count < limit;
            }

            _currentLobby = CSteamID.Nil;
            return false;
        }
        public static bool GetLobbyLink(out string link)
        {
            if (GetCurrentLobby(out _) && !string.IsNullOrEmpty(_lobbyLink))
            {
                link = _lobbyLink;
                return true;
            }

            link = "";
            return false;
        }
        [HarmonyPatch(nameof(SteamLobbyHandler.HandleMessage))]
        [HarmonyPostfix]
        static void Message(SteamLobbyHandler __instance, SteamLobbyHandler.MessageType messageType)
        {
            if (messageType == SteamLobbyHandler.MessageType.RoomID && __instance.InSteamLobby(out var lobby))
            {
                _currentLobby = lobby;
                __instance.LeaveLobby();
            }
        }
        [HarmonyPatch(nameof(SteamLobbyHandler.OnLobbyEnter))]
        [HarmonyPostfix]
        static void LobbyEnter(SteamLobbyHandler __instance, LobbyEnter_t param)
        {
            _currentLobby = new CSteamID(param.m_ulSteamIDLobby);
            _lobbyLink = $"steam://joinlobby/{SteamUtils.GetAppID()}/{_currentLobby}/{SteamMatchmaking.GetLobbyOwner(_currentLobby)}";

            GeneralUtil.DelayInvoke(() =>
            {
                if (__instance.InSteamLobby())
                {
                    __instance.LeaveLobby();
                    LogUtil.Log("Timeout reached on lobby");
                }
            }, 1.7f);
        }
        [HarmonyPatch(nameof(SteamLobbyHandler.OnLobbyDataUpdate))]
        [HarmonyPrefix]
        static bool UpdateCheck()
        {
            if (_block)
            {
                _block = false;
                return false;
            }

            return true;
        }
        [HarmonyPatch(nameof(SteamLobbyHandler.RequestPhotonRoomID))]
        [HarmonyPrefix]
        static bool DontSend(SteamLobbyHandler __instance)
        {
            __instance.m_currentlyRequestingRoomID = Optionable<CSteamID>.Some(__instance.m_currentLobby);
            return false;
        }
    }
}