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
            if (Character.localCharacter is Character c && c.data is CharacterData dat && dat.isClimbingAnything && c.refs.climbing is CharacterClimbing climb && c.input is CharacterInput input)
            {
                var move = input.movementInput;
                climb.playerSlide += move.normalized * 2f;
                dat.ropePercent += move.y / 7f;
            }
        }
    }
}