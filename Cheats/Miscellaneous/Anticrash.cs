using HarmonyLib;
using PeakCheat.Main;
using PeakCheat.Patches;
using PeakCheat.Types;
using PeakCheat.Utilities;
using UnityEngine;

namespace PeakCheat.Cheats.Miscellaneous
{
    [HarmonyPatch]
    internal class Anticrash: Cheat, PointerPatch
    {
        private static bool Patch => CheatHandler.IsEnabled<Anticrash>();
        public override string Name => "Crash Prevention";
        public override string Description => "Protects you from getting crashed by cheaters";
        public override bool DefaultEnabled => true;
        bool PointerPatch.Prefix(PointPinger pointer, Vector3 position) => TimeUtil.CheckTime(.1f) || !Patch;
        [HarmonyPatch(typeof(Dynamite), "RPC_Explode")]
        [HarmonyPrefix]
        static bool BombCrash() => TimeUtil.CheckTime(.5f) || !Patch;
        [HarmonyPatch(typeof(Constructable), "CreatePrefabRPC")]
        [HarmonyPrefix]
        static bool ConstructableCrash() => TimeUtil.CheckTime(.5f) || !Patch;
    }
}