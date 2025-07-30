using PeakCheat.Classes;
using PeakCheat.Utilities;
using Sirenix.Utilities;
using System.Linq;

namespace PeakCheat.Cheats.Abusive
{
    internal abstract class NearbyModule: Cheat
    {
        public virtual float Range => 5f;
        public virtual void Effect(CheatPlayer player) => player.ToString();
        public override void Method() => PlayerUtil.OtherPlayers().Where(P => P.InRange(Range)).ForEach(Effect);
    }
}
