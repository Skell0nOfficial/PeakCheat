using PeakCheat.Main;
using PeakCheat.Types;
using PeakCheat.Utilities;
using Photon.Pun;

namespace PeakCheat.Cheats.Miscellaneous
{
    internal class PeakCheatPromotion: Cheat
    {
        public override string Name => "PeakCheat Promo";
        public override string Description => "Spams the discord promo on everyone in the room";
        public override void Method()
        {
            if (PhotonNetwork.InRoom && TimeUtil.CheckTime(1f)) Promotion.SpamInvitation();
        }
    }
}
