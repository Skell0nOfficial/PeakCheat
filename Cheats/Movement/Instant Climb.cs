using PeakCheat.Types;

namespace PeakCheat.Cheats.Movement
{
    internal class SpeedyClimb: Cheat
    {
        public override string Name => "Speedy Climb";
        public override string Description => "Makes your climb speed faster";
        public override SceneType RequiredScene => SceneType.Airport;
        public override void Method()
        {
            var c = CheatPlayer.LocalPlayer.GameCharacter;
            if (c.data.isClimbingAnything)
            {
                c.refs.climbing.playerSlide += c.input.movementInput.normalized * 1.3f;
                Character.GainFullStamina();
            }
        }
    }
}
