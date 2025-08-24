using HarmonyLib;
using PeakCheat.Main;
using PeakCheat.Types;
using PeakCheat.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace PeakCheat.Patches
{
    public interface PointerPatch
    {
        public bool Prefix(PointPinger pointer, Vector3 position) => true;
        public void Postfix(PointPinger pointer, Vector3 position) { }
    }
    [HarmonyPatch(typeof(PointPinger), "ReceivePoint_Rpc")]
    internal class PointPatch: CheatBehaviour
    {
        private static readonly List<PointerPatch> _patches = new List<PointerPatch>();
        bool CheatBehaviour.DelayStart() => true;
        async void CheatBehaviour.Start()
        {
            await Task.Delay(3000);
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                if (type.GetInterface(nameof(PointerPatch)) != null)
                {
                    if (type.IsSubclassOf(typeof(CheatBehaviour)))
                    {
                        if (BehaviourHandler.TryGetBehaviour(type, out var c) && c is PointerPatch p) _patches.AddIfNew(p);
                        continue;
                    }
                    if (type.IsSubclassOf(typeof(Cheat)))
                    {
                        if (CheatHandler.TryGetCheat(type, out var c) && c is PointerPatch p) _patches.AddIfNew(p);
                        continue;
                    }
                    if (Activator.CreateInstance(type) is PointerPatch patch) _patches.AddIfNew(patch);
                }
        }
        public static bool Prefix(PointPinger __instance, Vector3 point, Vector3 hitNormal) => _patches.All(P => P.Prefix(__instance, point));
        public static void Postfix(PointPinger __instance, Vector3 point, Vector3 hitNormal) => _patches.ForEach(P => P.Postfix(__instance, point));
    }
}