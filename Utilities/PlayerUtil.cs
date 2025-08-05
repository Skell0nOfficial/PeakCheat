using PeakCheat.Classes;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        #endregion
        #region Scripts
        public static Vector3 NaN => Vector3.one * (float.MaxValue / 10f);
        public static void PlayerRPC(this CheatPlayer player, string RPC) => PlayerRPC(player, RPC, RpcTarget.All);
        public static void PlayerRPC(this CheatPlayer player, string RPC, RpcTarget target) => PlayerRPC(player, RPC, target, Array.Empty<object>());
        public static void PlayerRPC(this CheatPlayer player, string RPC, object data) => PlayerRPC(player, RPC, RpcTarget.All, data);
        public static void PlayerRPC(this CheatPlayer player, string RPC, CheatPlayer target, object data) => PlayerRPC(player, RPC, target, data);
        public static void PlayerRPC(this CheatPlayer player, string RPC, object[] data) => PlayerRPC(player, RPC, RpcTarget.All, data);
        public static void PlayerRPC(this CheatPlayer player, string RPC, RpcTarget target, params object[] data) => player.View?.RPC(RPC, target, data);
        public static void PlayerRPC(this CheatPlayer player, string RPC, CheatPlayer target, params object[] data) => player.View?.RPC(RPC, target, data);
        public static bool GetMaster() => PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient);
        public static void Kill(this CheatPlayer player) => PlayerRPC(player, "RPCA_Die", player.Position);
        public static async void Destroy(this CheatPlayer player)
        {
            if (!TimeUtil.CheckTime($"Destroy:{player.Name}", 3f)) return;
            if (!PhotonNetwork.IsMasterClient && !GetMaster())
            {
                LogUtil.Log(false, "[Destroy] Could not get master client!");
                return;
            }
            LogUtil.Log($"Destroy {player.ToString()}..");
            await Task.Delay(70);
            Faint(player);
            await Task.Delay(600);
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
        public static void SetDeadEyes(this CheatPlayer player, bool Dead) => PlayerRPC(player, Dead? "CharacterDied": "OnRevive_RPC");
        public static void Faint(this CheatPlayer player) => PlayerRPC(player, "RPCA_PassOut");
        public static void Fall(this CheatPlayer player) => PlayerRPC(player, "RPCA_Fall", 1f);
        public static void Revive(this CheatPlayer player)
        {
            if (!player.Dead)
            {
                LogUtil.Log(false, $"Attempted to Revive alive player \"{player.Name}\" ?");
                return;
            }
            LogUtil.Log($"Reviving Player \"{player.Name}\"..");
            PlayerRPC(player, "RPCA_ReviveAtPosition", new object[] { CheatPlayer.LocalPlayer.Position, true });
        }
        public static void FreezeMovement(this CheatPlayer player) => FreezeMovement(player, 1f);
        public static void FreezeMovement(this CheatPlayer player, float seconds)
        {
            if (!TimeUtil.CheckTime($"Freeze:{player.Name}", seconds)) return;
            for (float i = 0f; i <= seconds; i += .1f)
                GeneralUtil.DelayInvoke(() => SetVelocity(player, Vector3.zero), i);
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
            foreach (var part in PlayerHandler.GetPlayer(player).character.refs.ragdoll.partDict.Keys)
                PlayerRPC(player, "RPCA_AddForceToBodyPart", new object[] { part, Vector3.zero, velocity * 30f });
        }
        public static void Crash(this CheatPlayer player)
        {
            if (!TimeUtil.CheckTime(1f)) return;
        }
        #endregion
    }
}