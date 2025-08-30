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
        [PlayerScript("Kill")] public static void Kill(CheatPlayer player) => player.Kill();
        [PlayerScript("Clone Self")] public static void Clone(CheatPlayer player)
        {
            Exploits.ForceSetName(player, NetworkConnector.GetUsername());
            CustomCommands<CustomCommandType>.SendPackage(new SyncPersistentPlayerDataPackage
            {
                Data = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(PhotonNetwork.LocalPlayer),
                ActorNumber = player.PhotonPlayer?.ActorNumber ?? -1
            }, Photon.Realtime.ReceiverGroup.All);
        }
        [PlayerScript("Force Cheater")] public static void Cheater(CheatPlayer player) => Exploits.ForceSetProps(player, new ExitGames.Client.Photon.Hashtable()
        {
            ["AtlUser"] = PlayerFlagger.ForcedPropertyKey,
            ["CherryUser"] = PlayerFlagger.ForcedPropertyKey
        });
        [PlayerScript("Crash")] public static void Crash(CheatPlayer player) => player.Crash();
        [PlayerScript("Warp to Campfire")] public static void Campfire(CheatPlayer player)
        {
            if (CheatUtil.CurrentScene == SceneType.Airport)
            {
                LogUtil.Log($"Cant warp to Campfire in Airport");
                return;
            }
            if (UnityEngine.Object.FindObjectsByType<Campfire>(FindObjectsSortMode.None).Any(C => C != null && !C.Lit && C.state != global::Campfire.FireState.Spent, out var C))
            {
                player.Teleport(C.transform.position + (Vector3.back * 2f) + (Vector3.up * 3f));
                return;
            }
            LogUtil.Log(false, $"Cant find valid campfire?");
        }
        [PlayerScript("Warp to Player")] public static void Warp(CheatPlayer player) => CheatPlayer.LocalPlayer.Teleport(player.Head + (Vector3.up * 2f));
        [PlayerScript("Warp to Random")] public static void WarpRandom(CheatPlayer player) => player.Teleport(CheatPlayer.All.Where(C => C != player).ToArray().PickRandom().Head + (Vector3.up * 2f));
        [PlayerScript("Warp Player to you")] public static void TP(CheatPlayer player) => player.Teleport(CheatPlayer.LocalPlayer.Head + (Vector3.up * 2f));
        [PlayerScript("Break Game")] public static void FuckUpGame(CheatPlayer player)
        {
            player.SetVelocity(Vector3.up * 25f);
            player.BreakGame();
        }
        [PlayerScript("Revive")] public static void Revive(CheatPlayer player) => player.Revive();
        [PlayerScript("Speed Boost")] public static void Speed(CheatPlayer player) => player.FeedItem("Energy Drink");
        [PlayerScript("Force Fall")] public static void Fall(CheatPlayer player) => player.Fall(3f);
        [PlayerScript("Unfall")] public static void Unfall(CheatPlayer player) => player.Fall(-1f);
        [PlayerScript("Break Legs")] public static void BreakLegs(CheatPlayer player) => player.Fall(float.MaxValue);
        [PlayerScript("Force Faint")] public static void Sleep(CheatPlayer player) => player.Faint();
        [PlayerScript("Reset Statuses")] public static void Reset(CheatPlayer player) => player.ResetStatuses();
        [PlayerScript("Boost Status")] public static void Boost(CheatPlayer player)
        {
            Reset(player);
            player.SetMorale(float.MaxValue);
        }
        [PlayerScript("Give Unreleased Grappling Hook")] public static void Unreleased(CheatPlayer player) => player.GiveItem<RescueHook>();
        [PlayerScript("Trap")] public static void Trap(CheatPlayer player) => player.Trap();
    }
}