using PeakCheat.Types;
using Steamworks;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace PeakCheat.Utilities
{
    internal class ForceJoiner: CheatBehaviour
    {
        public enum JoinResult
        {
            Offline,
            Not_Peak,
            Main_Menu,
            Joining,
            Maybe_Joining
        };
        void UEJoinFriends()
        {
            void Log(object m) => LogUtil.Log(m.ToString());

            bool Joining = false;
            string Friend = string.Empty;
            var Flag = Steamworks.EFriendFlags.k_EFriendFlagAll;

            for (int i = 0; i < Steamworks.SteamFriends.GetFriendCount(Flag); i++)
            {
                var ID = Steamworks.SteamFriends.GetFriendByIndex(i, Flag);
                bool data = CanJoin(ID, out Friend, out string log);
                Debug.Log($"{log} ({(data ? "Success" : "Failure")})");
                if (data)
                {
                    Joining = true;
                    break;
                }
            }

            Log(Joining ? $"Joining {Friend}'s Lobby.." : "Couldnt find an available Steam Lobby");
        }
        async Task UEJoinRandom()
        {
            /*
                foreach (var c in PlayerHandler.GetAllPlayerCharacters())
                {
                    if (c.IsLocal) continue;
                    if (Vector3.Distance(c.Center, Camera.main.transform.position) >= 15f) continue;
                    PeakCheat.Utilities.PlayerUtil.Fling(c);
                }
            */

            void Log(object m) => LogUtil.Log(m.ToString());

            string step = "Getting lobby list";
            try
            {
                var lobbyList = await new System.Net.Http.HttpClient().GetStringAsync("https://gist.githubusercontent.com/Skell0nOfficial/5c8002560e48bd4d6587b4e2656dde2d/raw/PEAK_Lobbies.json");
                step = "Deserialing lobby list";
                var lobbies = Newtonsoft.Json.JsonConvert.DeserializeObject<ulong[]>(lobbyList);
                if (lobbies == null || lobbies.Length == 0)
                {
                    Log("No Lobbies Found");
                    return;
                }

                step = "Logging/Joining Lobby";
                Log(lobbies.Length != 10 ? $"Lobby Count: {lobbies.Length}" : "Got full lobby list");
                GameHandler.GetService<SteamLobbyHandler>().TryJoinLobby(new Steamworks.CSteamID(lobbies[UnityEngine.Random.Range(0, lobbies.Length)]));

                Log("Joining Lobby..");
            }
            catch (System.Exception Error)
            {
                Log($"Got error while joining ({step})\n\n{Error}");
            }
        }
        void UEUsings()
        {

        }
        void UEJoinSpecific()
        {
            void Log(object m) => LogUtil.Log(m.ToString());

            ulong lobbyID = 0;
            var ID = new Steamworks.CSteamID(lobbyID);
            var owner = Steamworks.SteamMatchmaking.GetLobbyOwner(ID);
            var count = Steamworks.SteamMatchmaking.GetNumLobbyMembers(ID);

            if (count <= 0)
            {
                Log("no members");
                return;
            }

            Log($"Joining \"{lobbyID}\" ({count} players)");
            Application.OpenURL($"steam://joinlobby/{Steamworks.SteamUtils.GetAppID().m_AppId}/{lobbyID}/{owner}");
        }
        void UETemp()
        {
            void Log(object m) => LogUtil.Log(m.ToString());

            var thing = Newtonsoft.Json.JsonConvert.DeserializeObject<ulong[]>(System.IO.File.ReadAllText("C:\\Users\\level\\source\\repos\\PEAK Lobby Finder\\bin\\Debug\\net9.0\\Lobbies.json")) ?? Array.Empty<ulong>();
            foreach (var lobbyID in thing)
            {
                var ID = new Steamworks.CSteamID(lobbyID);
                var owner = Steamworks.SteamMatchmaking.GetLobbyOwner(ID);
                var count = Steamworks.SteamMatchmaking.GetNumLobbyMembers(ID);
                string name = ID.ToString().Replace("109775244", "");

                if (count <= 0)
                {
                    Log($"[{name}] No Members");
                    continue;
                }

                Log($"[{name}] Joining, {count} Players Connected");
                Application.OpenURL($"steam://joinlobby/{Steamworks.SteamUtils.GetAppID().m_AppId}/{lobbyID}/{owner}");
            }
        }
        public bool CanJoin(CSteamID FriendID, out string Friend, out string Log)
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
        public bool CanJoin(CSteamID FriendID, out string Name, out JoinResult Log)
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
                Debug.Log($"Joining room [{URL}]");
                Application.OpenURL(URL);
                return true;
            }
            Log = JoinResult.Offline;
            return false;
        }
    }
}