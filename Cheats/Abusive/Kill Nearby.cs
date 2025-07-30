using PeakCheat.Classes;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal class KillNearby: NearbyModule
    {
        public override string Name => "Kill Nearby";
        public override string Description => "Kills anyone who comes to close";
        public override float Range => 2.7f;
        public override void Effect(CheatPlayer player) => player.Kill();
    }
}
