using PeakCheat.Types;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal class DisableItems: Cheat
    {
        public override string Name => "Disable Items";
        public override string Description => "Doesnt let anyone except you have an item";
        public override void Method()
        {
            foreach (var p in PlayerUtil.OtherPlayers())
                if (p.GetItem(out _))
                    p.DeleteItem();
        }
    }
}
