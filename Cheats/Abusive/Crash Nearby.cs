using PeakCheat.Classes;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal class CrashNearby: NearbyModule
    {
        public override string Name => "Crash Nearby";
        public override string Description => "Crashes anyone's game when they go too close";
        public override void Effect(CheatPlayer player) => player.Crash();
        public override float Range => 3f;
    }
}
