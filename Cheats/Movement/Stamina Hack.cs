using PeakCheat.Types;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Movement
{
    internal class StaminaHack: Cheat
    {
        public override string Name => "Stamina Hack";
        public override string Description => "Gives you infinite stamina";
        public override SceneType RequiredScene => SceneType.Airport;
        public override void Method()
        {
            Character.GainFullStamina();
            if (Character.localCharacter.refs.afflictions.statusSum != 0f)
                CheatPlayer.LocalPlayer.ResetStatuses();
        }
    }
}