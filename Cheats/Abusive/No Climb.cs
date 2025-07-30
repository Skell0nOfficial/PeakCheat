using PeakCheat.Classes;
using PeakCheat.Utilities;
using UnityEngine;

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
                {
                    player.Fall();
                    player.SetVelocity(Vector3.down * 30f);
                }
        }
    }
}