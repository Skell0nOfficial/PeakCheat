using PeakCheat.Main;
using PeakCheat.Types;
using UnityEngine;

namespace PeakCheat.Cheats.Miscellaneous
{
    internal class JoinDiscord: Cheat
    {
        public override string Name => "Join Discord";
        public override string Description => "Opens the discord link";
        public override void Enable()
        {
            Application.OpenURL(Promotion.DiscordLink);
            Enabled = false;
        }
    }
}