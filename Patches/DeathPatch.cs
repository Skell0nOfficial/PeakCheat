using HarmonyLib;
using PeakCheat.Utilities;
using Photon.Pun;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(Character), nameof(Character.RPCA_Die))]
    internal class DeathPatch
    {
        static void Prefix(Character __instance)
        {
            if (__instance.IsLocal && CheatUtil.CurrentScene != Types.SceneType.Level)
                GeneralUtil.DelayInvoke(() => __instance.photonView.RPC(nameof(Character.RPCA_ReviveAtPosition), RpcTarget.All, __instance.Center, true), .4f);
        }
    }
}