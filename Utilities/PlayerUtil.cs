using PeakCheat.Types;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zorro.Core;
using Zorro.PhotonUtility;

namespace PeakCheat.Utilities
{
    public static class PlayerUtil
    {
        #region Utilities
        private static readonly Dictionary<global::Player, CheatPlayer> _players = new Dictionary<global::Player, CheatPlayer>();
        public static CheatPlayer[] AllPlayers() => PlayerHandler.GetAllPlayers().Select(ToCheatPlayer).ToArray();
        public static CheatPlayer[] OtherPlayers() => AllPlayers().Where(P => !P.PhotonPlayer.IsLocal).ToArray();
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
        public static Vector3 Position(this CheatPlayer player) => player.Center;
        public static bool InRange(this CheatPlayer player, float range) =>
            Vector3.Distance(UnityUtil.CurrentPosition(), player.Position()) <= range;
        public static bool TryGetPhotonPlayer(this global::Player player, out Photon.Realtime.Player? photonPlayer)
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
        public static int GetID(this CheatPlayer player) => GeneralUtil.Compute(player.Name + (player.PhotonPlayer?.ActorNumber ?? -1) + (PhotonNetwork.CurrentRoom?.Name ?? "null"));
        #endregion
        #region Scripts
        public static Vector3 NaN => Vector3.one * float.NaN;
        public static Vector3 FarAway => Vector3.one * (float.MaxValue / 27f);
        public static Vector3 GetRespawnPosition()
        {
            if (Singleton<MapHandler>.Instance is MapHandler map)
            {
                return map.segments[(int)map.GetCurrentSegment()]?.reconnectSpawnPos.position ?? Vector3.zero;
            }
            
            return Character.AllCharacters.Any(C => !C.data.dead, out var character)? character.Head + (Vector3.up * 2f): Vector3.zero;
        }
        public static void PlayerRPC(this CheatPlayer player, string RPC) => PlayerRPC(player, RPC, RpcTarget.All);
        public static void PlayerRPC(this CheatPlayer player, string RPC, RpcTarget target) => PlayerRPC(player, RPC, target, Array.Empty<object>());
        public static void PlayerRPC(this CheatPlayer player, string RPC, object data) => PlayerRPC(player, RPC, RpcTarget.All, data);
        public static void PlayerRPC(this CheatPlayer player, string RPC, CheatPlayer target, object data) => PlayerRPC(player, RPC, target, data.SingleArray());
        public static void PlayerRPC(this CheatPlayer player, string RPC, params object[] data) => PlayerRPC(player, RPC, RpcTarget.All, data);
        public static void PlayerRPC(this CheatPlayer player, string RPC, RpcTarget target, params object[] data) => player.View?.RPC(RPC, target, data);
        public static void PlayerRPC(this CheatPlayer player, string RPC, CheatPlayer target, params object[] data) => player.View?.RPC(RPC, target, data);
        public static void Kill(this CheatPlayer player)
        {
            if (player.Dead) return;
            if (!player.AnticheatUser)
            {
                PlayerRPC(player, "RPCA_Die", player.Center);
                return;
            }
            for (int i = 0; i < 5; i++) PrefabUtil.SummonExplosion(player.Position());
        }
        public static void FeedItem<T>(this CheatPlayer player) where T : Item
        {
            if (!Exploits.TryGetPrefab<T>(out var obj) || obj == null)
            {
                LogUtil.Log($"Failed to get prefab for [{typeof(T).Name}]");
                return;
            }

            FeedItem(player, obj.name);
        }
        public static void FeedItem(this CheatPlayer player, string name) => ForceFeedUtil.ForceFeed(player, name.Replace("(Clone)", ""));
        public static void GiveItem<T>(this CheatPlayer player) where T : Component
        {
            if (!Exploits.TryGetPrefab<T>(out var obj) || obj == null)
            {
                LogUtil.Log($"Failed to get prefab for [{typeof(T).Name}]");
                return;
            }

            GiveItem(player, obj.name.Replace("(Clone)", ""));
        }
        public static void GiveItem(this CheatPlayer player, string name) => GameUtils.instance.photonView.RPC("InstantiateAndGrabRPC", RpcTarget.MasterClient, name, player.View);
        public static void SlowKill(this CheatPlayer player)
        {
            if (!player.AnticheatUser)
            {
                SetAllStatuses(player, 1.6F / Enum.GetValues(typeof(CharacterAfflictions.STATUSTYPE)).Length);
                return;
            }
            LogUtil.Log($"Blocked AntiCheat user: {player.Name}");
        }
        public static void SpazCosmetics()
        {
            float perSecond = 2f;
            float seconds = 5f;

            for (float i = 1f; i <= seconds * perSecond; i++)
                GeneralUtil.DelayInvoke(() =>
                {
                    foreach (var p in PhotonNetwork.PlayerListOthers)
                    {
                        CharacterCustomization.Randomize();
                        CustomCommands<CustomCommandType>.SendPackage(new SyncPersistentPlayerDataPackage
                        {
                            Data = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(PhotonNetwork.LocalPlayer),
                            ActorNumber = p.ActorNumber
                        }, ReceiverGroup.All);
                    }
                }, 1 / perSecond * i);
        }
        public static void Fling(this CheatPlayer player)
        {
            if (!TimeUtil.CheckTime($"Fling:{player.Name}", 2f))
            {
                LogUtil.Log(false, "Rejecting fling (time delay returned false)");
                return;
            }

            for (int i = 0; i < 50; i++) SetVelocity(player, UnityUtil.RandomDirection() * (i + 1));
        }
        public static void Jump(this CheatPlayer player) => Jump(player, true, true);
        public static void Jump(this CheatPlayer player, bool ForceJump, bool PalJump)
        {
            if (player.AnticheatUser)
            {
                LogUtil.Log($"Blocked AntiCheat user: {player.Name}");
                return;
            }
            if (ForceJump)
                PlayerRPC(player, "JumpRpc", player, PalJump);
            else if (player.OnGround) PlayerRPC(player, "JumpRpc", player, PalJump);
        }
        public static void FakeKill(this CheatPlayer player) => SetDeadEyes(player, true);
        public static void FakeRevive(this CheatPlayer player) => SetDeadEyes(player, false);
        public static void SetDeadEyes(this CheatPlayer player, bool Dead) => PlayerRPC(player, Dead ? "CharacterDied" : "OnRevive_RPC");
        public static void Faint(this CheatPlayer player) => PlayerRPC(player, "RPCA_PassOut", player);
        public static void Fall(this CheatPlayer player, float seconds)
        {
            if (seconds > 0f)
            {
                PlayerRPC(player, "RPCA_Fall", player, seconds);
                return;
            }

            PlayerRPC(player, "RPCA_UnFall");
        }
        public static void Revive(this CheatPlayer player)
        {
            if (player.AnticheatUser)
            {
                LogUtil.Log($"Blocked AntiCheat user: {player.Name}");
                return;
            }

            if (player.Alive)
            {
                if (player.PassedOut) PlayerRPC(player, "RPCA_UnPassOut");
                return;
            }

            PlayerRPC(player, "RPCA_ReviveAtPosition", new object[] { GetRespawnPosition(), true });
        }
        public static void DeleteItem(this CheatPlayer player)
        {
            if (player.AnticheatUser)
            {
                LogUtil.Log($"Blocked AntiCheat user: {player.Name}");
                return;
            }

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
        }
        public static void Teleport(this CheatPlayer player, Vector3 pos)
        {
            if (player.AnticheatUser)
            {
                LogUtil.Log($"Blocked AntiCheat user: {player.Name}");
                return;
            }
            PlayerRPC(player, "WarpPlayerRPC", player, new object[] { pos, true });
        }
        public static void Trap(this CheatPlayer player)
        {
            if (!Exploits.TryGetPrefab<MagicBean>(out var obj) || obj == null) return;
            if (!PhotonNetwork.InstantiateItem(obj.name, FarAway, Quaternion.identity).TryGetComponent<PhotonView>(out var viewObj)) return;

            if (!player.AnticheatUser) Fall(player, .4f);
            if (viewObj is PhotonView view)
                foreach (var dir in UnityUtil.GetDirections())
                {
                    view.RPC("GrowVineRPC", player.PhotonPlayer, player.Center + (dir.normalized * 2f), dir, 3f);
                    foreach (var dir2 in UnityUtil.GetDirections()) view.RPC("GrowVineRPC", player.PhotonPlayer, player.Center + ((dir2 + dir).normalized * 2f), dir2, 3f);
                }
        }
        public static void SpazScreen(this CheatPlayer player)
        {
            if (!player.AnticheatUser)
            {
                PlayerRPC(player, "RPCA_FallWithScreenShake", player, 1f, float.MaxValue);
                return;
            }
            LogUtil.Log($"Blocked AntiCheat user: {player.Name}");
        }
        public static void SetMorale(this CheatPlayer player, float morale) => PlayerRPC(player, "MoraleBoost", morale, 0);
        public static void SetVelocity(this CheatPlayer player, Vector3 velocity)
        {
            foreach (var part in player.GameCharacter.refs.ragdoll.partList)
                PlayerRPC(player, "RPCA_AddForceToBodyPart", player, new object[] { part.partType, Vector3.zero, velocity * 30f });
        }
        public static void BreakGame(this CheatPlayer player)
        {
            int num = 0;
            var RPCMethod = typeof(CharacterData).GetMethod("RPC_SyncOnJoin");
            var parameters = new object[RPCMethod.GetParameters().Length];

            foreach (var param in RPCMethod.GetParameters())
            {
                var paramType = param.ParameterType;
                if (paramType == typeof(bool)) parameters[num++] = !param.Name.Contains("dead");
                else if (paramType == typeof(bool[])) parameters[num++] = Array.Empty<bool>();
                else if (paramType == typeof(PhotonView)) parameters[num++] = null;
                else parameters[num++] = Activator.CreateInstance(paramType);
            }

            PlayerRPC(player, "RPC_SyncOnJoin", player, parameters);
        }
        public static void ResetStatuses(this CheatPlayer player) => SetAllStatuses(player, 0f);
        public static void SetStatus(this CheatPlayer player, CharacterAfflictions.STATUSTYPE status, float value)
        {
            var values = player.GameCharacter?.refs.afflictions?.currentStatuses ?? Array.Empty<float>();
            if (values.Length == 0) return;
            var array = new float[values.Length];
            for (int i = 0; i < values.Length; i++)
                array[i] = (i == (int)status) ? value : values[i];
            PlayerRPC(player, "ApplyStatusesFromFloatArrayRPC", array);
        }
        public static void SetAllStatuses(this CheatPlayer player, float value)
        {
            var values = player.GameCharacter?.refs.afflictions?.currentStatuses ?? Array.Empty<float>();
            if (values.Length == 0) return;
            PlayerRPC(player, "ApplyStatusesFromFloatArrayRPC", values.Select(V => value).ToArray());
        }
        public static async void Crash(this CheatPlayer player)
        {

            if (!TimeUtil.CheckTime($"Crash:{player.Name}", 3f)) return;
            if (!Exploits.TryGetPrefab<Dynamite>(out var obj) || obj == null) return;
            if (!PhotonNetwork.InstantiateItem(obj.name, FarAway, Quaternion.identity).TryGetComponent<PhotonView>(out var view)) return;
            
            view.RPC("SetKinematicRPC", player.PhotonPlayer, true, player.Position(), Quaternion.identity);
            await Task.Delay(200);
            int power = 1080;
            for (int i = 0; i < power; i++)
            {
                if (i == (power - (power / 10))) view.RPC("SetKinematicRPC", player.PhotonPlayer, true, NaN, Quaternion.identity);
                view.RPC("RPC_Explode", player.PhotonPlayer);
            }
            
            await Task.Delay(1000);
            for (int i = 0; i < 10; i++) PhotonNetwork.SendAllOutgoingCommands();

            LogUtil.Log($"Sent Crash Payload :: {Time.frameCount / PhotonNetwork.GetPing()}");
            PhotonNetwork.Destroy(view);
        }
        public static void Destroy(this CheatPlayer player) => PhotonNetwork.DestroyPlayerObjects(player.Actor?? -1, false);
        public static void BombPlayer(this CheatPlayer player) => BombPlayers(player.SingleArray());
        public static void BombPlayers(this CheatPlayer[] players)
        {
            if (!Exploits.TryGetPrefab<Dynamite>(out var obj) || obj == null)
            {
                LogUtil.Log(false, "Cant find Dynamite object?");
                return;
            }
            if (!PhotonNetwork.InstantiateItem(obj.name, Vector3.zero, Quaternion.identity).TryGetComponent<PhotonView>(out var viewObj))
            {
                LogUtil.Log(false, "Cant get Dynamite view component?");
                return;
            }

            if (viewObj is PhotonView view)
                foreach (var player in players)
                    if (player.PhotonPlayer is Photon.Realtime.Player p)
                    {
                        view.RPC("SetKinematicRPC", p, true, player.Position(), Quaternion.identity);
                        GeneralUtil.DelayInvoke(() => view.RPC("RPC_Explode", p), .07f);
                    }
                    else LogUtil.Log(false, "PhotonPlayer is null?");
            else LogUtil.Log(false, "viewObj is null?");
        }
        #endregion
    }
}