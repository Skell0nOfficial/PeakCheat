using PeakCheat.Types;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Visuals
{
    internal class DeadEyes: Cheat
    {
        public override string Name => "Dead Eyes";
        public override string Description => "Makes your eyes cross as if you were dead";
        public override SceneType RequiredScene => SceneType.Airport;
        public override void Method()
        {
            if (TimeUtil.CheckTime(1f)) CheatPlayer.LocalPlayer.SetDeadEyes(true);
        }
        public override void Disable() => CheatPlayer.LocalPlayer.SetDeadEyes(CheatPlayer.LocalPlayer.Dead);
    }
}
