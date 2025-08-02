using PeakCheat.Classes;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal class FreezeAll: AllModule
    {
        public override string Name => "Freeze All";
        public override string Description => "Freezes all players movement for 1 second";
        public override void Effect(CheatPlayer player) => player.FreezeMovement();
    }
}
