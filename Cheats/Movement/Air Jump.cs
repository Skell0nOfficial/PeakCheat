using PeakCheat.Classes;
using PeakCheat.Utilities;
using UnityEngine;

namespace PeakCheat.Cheats.Movement
{
    internal class AirJump: Cheat
    {
        public override string Name => "Air Jump";
        public override string Description => "Lets your jump even while in the air";
        public override void Method()
        {
            if (!UnityUtil.OnGround() && Input.GetKeyDown(KeyCode.Space))
                PlayerUtil.PlayerRPC(CheatPlayer.LocalPlayer, "JumpRpc", false);
        }
    }
}