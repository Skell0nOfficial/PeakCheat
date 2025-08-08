using PeakCheat.Classes;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Abusive
{
    internal class NoClimb: Cheat
    {
        public override string Name => "No Climb";
        public override string Description => "Doesn't allow anyone to climb except you";
        public override void Method()
        {
            foreach (var player in PlayerUtil.OtherPlayers())
                if (player.GameCharacter.data.isClimbingAnything)
                    player.Fall(.1f);
        }
    }
}