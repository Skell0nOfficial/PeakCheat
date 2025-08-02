using PeakCheat.Classes;
using PeakCheat.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakCheat.Cheats.Abusive
{
    internal class DestroyNearby: NearbyModule
    {
        public override string Name => "Destroy Nearby";
        public override string Description => "Makes whoever goes near you pass out, then destroys them";
        public override void Effect(CheatPlayer player) => player.Destroy();
    }
}
