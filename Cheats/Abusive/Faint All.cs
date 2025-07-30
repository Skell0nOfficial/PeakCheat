using PeakCheat.Classes;
using PeakCheat.Utilities;
using Sirenix.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal class FaintAll: AllModule
    {
        public override string Name => "Faint All";
        public override string Description => "Makes everyone in the room go to sleep";
        public override void Effect(CheatPlayer player) => player.Faint();
    }
}