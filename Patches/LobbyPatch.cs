using HarmonyLib;
using Steamworks;
using Zorro.Core.Serizalization;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(SteamLobbyHandler), nameof(SteamLobbyHandler.HandleMessage))]
    internal class LobbyPatch
    {
        static void Postfix(SteamLobbyHandler __instance, SteamLobbyHandler.MessageType messageType, BinaryDeserializer deserializer, CSteamID lobbyID)
        {
            if (messageType == SteamLobbyHandler.MessageType.RoomID && __instance.InSteamLobby())
                __instance.LeaveLobby();
        }
    }
}