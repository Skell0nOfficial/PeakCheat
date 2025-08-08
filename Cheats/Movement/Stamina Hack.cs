using PeakCheat.Classes;

namespace PeakCheat.Cheats.Movement
{
    internal class StaminaHack: Cheat
    {
        public override string Name => "Stamina Hack";
        public override string Description => "Gives you infinite stamina";
        public override void Method() => Character.GainFullStamina();
    }
}