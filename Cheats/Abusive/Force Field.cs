using PeakCheat.Classes;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal class ForceField: NearbyModule
    {
        public override string Name => "Force Field";
        public override string Description => "Pushes anyone who comes too close to you";
        public override void Effect(CheatPlayer player) => player.SetVelocity((player.Position() - UnityUtil.CurrentPosition()).normalized * 17f);
    }
}
