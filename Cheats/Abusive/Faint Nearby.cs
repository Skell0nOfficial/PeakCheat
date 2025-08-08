using PeakCheat.Classes;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal class FaintNearby: NearbyModule
    {
        public override string Name => "Faint Nearby";
        public override string Description => "If someone goes near you, they faint";
        public override void Effect(CheatPlayer player) => player.Faint();
    }
}