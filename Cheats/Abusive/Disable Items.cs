using PeakCheat.Classes;
using PeakCheat.Utilities;
using System.Linq;

namespace PeakCheat.Cheats.Abusive
{
    internal class DisableItems: Cheat
    {
        public override string Name => "Disable Items";
        public override string Description => "Doesnt let anyone (except you) use any items";
        private static readonly Item.ItemTags[] _allowedTags =
        {
            Item.ItemTags.None,
            Item.ItemTags.BingBong
        };
        public override void Method()
        {
            foreach (var player in PlayerUtil.OtherPlayers())
                if (player.GetItem(out var item) && item != null && _allowedTags.All(tag => !item.itemTags.HasFlag(tag)))
                    player.DeleteItem();
        }
    }
}
