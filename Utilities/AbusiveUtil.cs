using PeakCheat.Classes;
using Photon.Pun;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public static class AbusiveUtil
    {
        public static Vector3 NaN => Vector3.one * (float.MaxValue / 10f);
        private static void PlayerRPC(this CheatPlayer player, string RPC) => PlayerRPC(player, RPC, RpcTarget.All);
        private static void PlayerRPC(this CheatPlayer player, string RPC, RpcTarget target) => PlayerRPC(player, RPC, target, Array.Empty<object>());
        private static void PlayerRPC(this CheatPlayer player, string RPC, object data) => PlayerRPC(player, RPC, RpcTarget.All, new object[] { data });
        private static void PlayerRPC(this CheatPlayer player, string RPC, CheatPlayer target, object data) => PlayerRPC(player, RPC, target, new object[] {data});
        private static void PlayerRPC(this CheatPlayer player, string RPC, object[] data) => PlayerRPC(player, RPC, RpcTarget.All, data);
        private static void PlayerRPC(this CheatPlayer player, string RPC, RpcTarget target, object[] data) => player.View?.RPC(RPC, target, data);
        private static void PlayerRPC(this CheatPlayer player, string RPC, CheatPlayer target, object[] data) => player.View?.RPC(RPC, target, data);
        public static bool GetMaster() => PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient);
        public static void LogAssembly() => Debug.Log(string.Join('\n', new System.Diagnostics.StackTrace().GetFrames().Select(F => $"{F.GetType().Assembly}:{F.GetMethod().Name}")));
        public static async void Kill(this CheatPlayer player)
        {
            if (!PhotonNetwork.IsMasterClient && !GetMaster())
            {
                LogUtil.Log(false, "[Freeze] Could not get master client!");
                return;
            }
            LogUtil.Log($"Freezing {player.ToString()}..");
            await Task.Delay(70);
            Faint(player);
            await Task.Delay(600);
            PhotonNetwork.DestroyPlayerObjects(player.PhotonPlayer.ActorNumber, false);
        }
        public static void Faint(this CheatPlayer player) => PlayerRPC(player, "RPCA_PassOut");
        public static void Fall(this CheatPlayer player) => PlayerRPC(player, "RPCA_Fall", 1f);
        public static void Revive(this CheatPlayer player)
        {
            if (player.GameCharacter.data.dead)
                PlayerRPC(player.PhotonPlayer, "RPCA_Revive", true);
        }
        public static void KillTP(this CheatPlayer player, Vector3 pos)
        {
            PlayerRPC(player, "RPCA_Die", Vector3.zero);
            PlayerRPC(player, "RPCA_ReviveAtPosition", new object[] {pos, true});
            PlayerRPC(player, "RPCA_Die", Vector3.zero);
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
        public static void SpazScreen(this CheatPlayer player) => PlayerRPC(player, "RPCA_FallWithScreenShake", player, new object[] {.1f, float.MaxValue});
        public static void SetVelocity(this CheatPlayer player, Vector3 velocity)
        {
            foreach (var part in PlayerHandler.GetPlayer(player).character.refs.ragdoll.partDict.Keys)
                PlayerRPC(player, "RPCA_AddForceToBodyPart", new object[] { part, Vector3.zero, velocity * 30f });
        }
        public static void Crash(this CheatPlayer player) => Teleport(player, NaN);
    }
}