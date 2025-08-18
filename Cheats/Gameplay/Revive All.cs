using PeakCheat.Types;
using PeakCheat.Utilities;
using Sirenix.Utilities;
using System.Linq;

namespace PeakCheat.Cheats.Gameplay
{
    internal class ReviveAll : Cheat
    {
        public override string Name => "Revive All";
        public override string Description => "Revives every dead player";
        public override SceneType RequiredScene => SceneType.Airport;
        public override void Enable()
        {
            if (!TimeUtil.CheckTime(1f)) return;
            PlayerUtil.AllPlayers().Where(P => P.Dead).ForEach(PlayerUtil.Revive);
            Enabled = false;
        }
    }
}