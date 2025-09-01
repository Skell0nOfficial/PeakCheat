using HarmonyLib;
using PeakCheat.Main;
using PeakCheat.Types;
using Photon.Pun;

namespace PeakCheat.Cheats.Miscellaneous
{
    [HarmonyPatch]
    internal class StartGame: Cheat
    {
        public override string Name => "Force Starter";
        public override string Description => "Always lets you start the game in the airport";
        [HarmonyPatch(typeof(BoardingPass), "StartGame")]
        [HarmonyPrefix]
        static bool Patch(BoardingPass __instance)
        {
            if (CheatHandler.IsEnabled<StartGame>())
            {
                __instance.kiosk.photonView.RPC("BeginIslandLoadRPC", RpcTarget.All, "WilIsland", __instance.ascentIndex);
                return false;
            }
            return true;
        }
    }
}
