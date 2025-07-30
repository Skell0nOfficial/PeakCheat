using PeakCheat.Classes;
using UnityEngine;

namespace PeakCheat.Cheats.Movement
{
    internal class InstantClimb: Cheat
    {
        public override string Name => "Insta Climb";
        public override string Description => "Makes you climb instantly!";
        public override void Method()
        {
            var c = CheatPlayer.LocalPlayer.GameCharacter;
            if (c.data.isClimbingAnything)
            {
                c.refs.climbing.playerSlide += c.input.movementInput.normalized * 7f;
                Character.GainFullStamina();
            }
        }
    }
}
