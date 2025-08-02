using PeakCheat.Classes;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal class DestroyAll: AllModule
    {
        public override string Name => "Destroy All";
        public override string Description => "Makes everyone pass out, then destroys them";
        public override void Effect(CheatPlayer player) => player.Destroy();
    }
}
