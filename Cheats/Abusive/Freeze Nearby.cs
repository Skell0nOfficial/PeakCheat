using PeakCheat.Classes;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal class FreezeNearby : AllModule
    {
        public override string Name => "Freeze Nearby";
        public override string Description => "Freezes whoever comes near you for 1 second";
        public override void Effect(CheatPlayer player) => player.FreezeMovement();
    }
}
