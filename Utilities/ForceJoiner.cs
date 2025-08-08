using PeakCheat.Classes;
using Steamworks;
using UnityEngine;

namespace PeakCheat.Utilities
{
    internal class ForceJoiner: UITab, CheatBehaviour
    {
        public override string Name => "Room Joiner";
        public enum JoinResult
        {
            Offline,
            Not_Peak,
            Main_Menu,
            Joining,
            Maybe_Joining
        };
        void UnityExplorerTester()
        {
            void Log(object m) {}

            bool Joining = false;
            string Friend = string.Empty;
            var Flag = Steamworks.EFriendFlags.k_EFriendFlagAll;

            for (int i = 0; i < Steamworks.SteamFriends.GetFriendCount(Flag); i++)
            {
                var ID = Steamworks.SteamFriends.GetFriendByIndex(i, Flag);
                bool data = PeakCheat.Utilities.ForceJoiner.CanJoin(ID, out Friend, out string log);
                Debug.Log($"{log} ({(data ? "Success" : "Failure")})");
                if (data)
                {
                    Joining = true;
                    break;
                }
            }

            Log(Joining ? $"Joining {Friend}'s Lobby.." : "Couldnt find an available Steam Lobby");
        }
        public static bool CanJoin(CSteamID FriendID, out string Friend, out string Log)
        {
            bool result = CanJoin(FriendID, out string Nickname, out JoinResult joinResult);

            Friend = Nickname;
            Log = joinResult switch
            {
                JoinResult.Offline => $"[{Friend}] Not Connected to Steam",
                JoinResult.Not_Peak => $"[{Friend}] Not Playing PEAK",
                JoinResult.Main_Menu => $"[{Friend}] Not Connected to a Lobby",
                JoinResult.Maybe_Joining => $"[{Friend}] Attempting to Join Lobby (70% Chance)",
                JoinResult.Joining => $"[{Friend}] Joining Lobby",
                _ => $"Unknown Result ({joinResult.ToString().Replace('_', ' ')})"
            };

            return result;
        }
        public static bool CanJoin(CSteamID FriendID, out string Name, out JoinResult Log)
        {
            Name = SteamFriends.GetFriendPersonaName(FriendID);
            if (SteamFriends.GetFriendGamePlayed(FriendID, out var dat))
            {
                var Game = dat.m_gameID.AppID().m_AppId;
                if (SteamUtils.GetAppID().m_AppId != Game)
                {
                    Log = JoinResult.Not_Peak;
                    return false;
                }
                var Lobby = dat.m_steamIDLobby;
                if (CSteamID.Nil == Lobby)
                {
                    Log = JoinResult.Main_Menu;
                    return false;
                }
                Log = JoinResult.Joining;
                var Host = SteamMatchmaking.GetLobbyOwner(Lobby);
                if (CSteamID.Nil == Host) Log = JoinResult.Maybe_Joining;
                string URL = $"steam://joinlobby/{Game}/{Lobby}/{Host}";
                Application.OpenURL(URL);
                return true;
            }
            Log = JoinResult.Offline;
            return false;
        }
    }
}
