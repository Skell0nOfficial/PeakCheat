using PeakCheat.Types;
using PeakCheat.Utilities;
using UnityEngine;

namespace PeakCheat.Cheats.Movement
{
    internal class AirJump: Cheat
    {
        public override string Name => "Air Jump";
        public override string Description => "Lets your jump even while in the air";
        public override SceneType RequiredScene => SceneType.Airport;
        public override void Method()
        {
            if (!UnityUtil.OnGround())
            {
                Character.GainFullStamina();
                if (Input.GetKey(KeyCode.Space) && TimeUtil.CheckTime(.5f))
                {
                    Character.localCharacter.data.sinceGrounded = -.1f;
                    PlayerUtil.Jump(CheatPlayer.LocalPlayer, true, false);
                }
            }
        }
    }
}