using PeakCheat.Main;
using PeakCheat.Patches;
using PeakCheat.Types;
using PeakCheat.Utilities;
using UnityEngine;

namespace PeakCheat.Cheats.Fun
{
    internal class ExplosivePoints: Cheat, PointerPatch
    {
        public override string Name => "Explosive Points";
        public override string Description => "Lets you spawn explosions where ever you point";
        public override SceneType RequiredScene => SceneType.Airport;
        public override void Method()
        {
            if (CheatPlayer.LocalPlayer.Dead && Character.localCharacter.input.pingWasPressed)
                PrefabUtil.SummonExplosion(Character.observedCharacter.Head);
        }
        bool PointerPatch.Prefix(PointPinger pointer, Vector3 pos)
        {
            if (pointer.TryGetComponent<Character>(out var c) && c.IsLocal && CheatHandler.IsEnabled<ExplosivePoints>())
            {
                GeneralUtil.DelayInvoke(() => PrefabUtil.SummonExplosion(pos), .4f);
                return false;
            }
            return true;
        }
    }
}