using PeakCheat.Classes;
using PeakCheat.Utilities;
using Sirenix.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal class KillAll: AllModule
    {
        public override string Name => "Kill All";
        public override string Description => "Makes everyone faint then die forever";
        public override void Effect(CheatPlayer player) => player.Kill();
    }
}
