using PeakCheat.Types;
using PeakCheat.Utilities;
using Photon.Pun;
using System.Linq;
using UnityEngine;
using Zorro.PhotonUtility;

namespace PeakCheat.Main
{
    [ScriptBase]
    internal class Scripts
    {
        [PlayerScript("TP")] public static void TP(CheatPlayer player) => player.Teleport(CheatPlayer.LocalPlayer.Head + Vector3.up * 2f);
        [PlayerScript("Fall")] public static void Fall(CheatPlayer player) => player.Fall(3f);
        [PlayerScript("Kill")] public static void Kill(CheatPlayer player) => player.Kill();
        [PlayerScript("Trap")] public static void Trap(CheatPlayer player) => player.Trap();
        [PlayerScript("Warp")] public static void Warp(CheatPlayer player) => CheatPlayer.LocalPlayer.Teleport(player.Head + Vector3.up * 2f);
        [PlayerScript("Bomb")] public static void Bomb(CheatPlayer player) => player.BombPlayer();
        [PlayerScript("Crash")] public static void Crash(CheatPlayer player) => player.Crash();
        [PlayerScript("Sleep")] public static void Sleep(CheatPlayer player) => player.Faint();
        [PlayerScript("Revive")] public static void Revive(CheatPlayer player) => player.Revive();
        [PlayerScript("Cheater")] public static void Cheater(CheatPlayer player) => player.ForceSetProps(new ExitGames.Client.Photon.Hashtable()
        {
            ["AtlUser"] = PlayerFlagger.ForcedPropertyKey,
            ["CherryUser"] = PlayerFlagger.ForcedPropertyKey
        });
        [PlayerScript("Unfall")] public static void Unfall(CheatPlayer player) => player.Fall(-1f);
        [PlayerScript("Clone Self")] public static void Clone(CheatPlayer player)
        {
            player.ForceSetName(NetworkConnector.GetUsername());
            CustomCommands<CustomCommandType>.SendPackage(new SyncPersistentPlayerDataPackage
            {
                Data = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(PhotonNetwork.LocalPlayer),
                ActorNumber = player.PhotonPlayer?.ActorNumber ?? -1
            }, Photon.Realtime.ReceiverGroup.All);
        }
        [PlayerScript("Boost Status")] public static void Boost(CheatPlayer player)
        {
            Reset(player);
            player.SetMorale(float.MaxValue);
        }
        [PlayerScript("Break Legs")] public static void BreakLegs(CheatPlayer player) => player.Fall(float.MaxValue);
        [PlayerScript("Reset Statuses")] public static void Reset(CheatPlayer player) => player.ResetStatuses();
        [PlayerScript("Speed Boost")] public static void Speed(CheatPlayer player) => player.FeedItem("Energy Drink");
        [PlayerScript("Warp to Random")] public static void WarpRandom(CheatPlayer player) => player.Teleport(CheatPlayer.All.Where(C => C != player).ToArray().PickRandom().Head + Vector3.up * 2f);
        [PlayerScript("Break Game")] public static void FuckUpGame(CheatPlayer player)
        {
            if (Object.FindObjectsByType<Campfire>(FindObjectsSortMode.None).Any(C => C != null, out var c))
                for (int i = 0; i < 300; i++)
                    c.SetFireWoodCount(i + 1);
            player.SetVelocity(Vector3.up * 25f);
            player.BreakGame();
        }
        [PlayerScript("Destroy Objects")] public static void Destroy(CheatPlayer player) => player.Destroy();
        [PlayerScript("Warp to Campfire")] public static void Campfire(CheatPlayer player)
        {
            if (CheatUtil.CurrentScene == SceneType.Airport)
            {
                LogUtil.Log($"Cant warp to Campfire in Airport");
                return;
            }
            if (Object.FindObjectsByType<Campfire>(FindObjectsSortMode.None).Any(C => C != null && !C.Lit && C.state != global::Campfire.FireState.Spent, out var C))
            {
                player.Teleport(C.transform.position + Vector3.back * 2f + Vector3.up * 3f);
                return;
            }
            LogUtil.Log(false, $"Cant find valid campfire?");
        }
        [PlayerScript("Give Unreleased Grappling Hook")] public static void Unreleased(CheatPlayer player) => player.GiveItem<RescueHook>();

    }
}