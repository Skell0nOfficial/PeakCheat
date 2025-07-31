using PeakCheat.Classes;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Gameplay
{
    internal class Immortality: Cheat
    {
        public override string Name => "Immortality";
        public override string Description => "Grants you Immortality";
        public override void Method() => CheatPlayer.LocalPlayer.Revive();
    }
}