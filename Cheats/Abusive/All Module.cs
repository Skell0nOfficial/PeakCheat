using PeakCheat.Classes;
using PeakCheat.Utilities;
using Sirenix.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal abstract class AllModule: Cheat
    {
        public virtual void Effect(CheatPlayer player) => player.ToString();
        public override void Enable()
        {
            PlayerUtil.OtherPlayers().ForEach(P => Effect(P));
            Enabled = false;
        }
    }
}
