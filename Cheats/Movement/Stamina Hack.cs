using PeakCheat.Classes;
using Photon.Voice.Unity;
using System;

namespace PeakCheat.Cheats.Movement
{
    internal class StaminaHack: Cheat
    {
        public override string Name => "Stamina Hack";
        public override string Description => "Gives you infinite stamina";
        /*
        var c = Character.localCharacter;

            var view = c.photonView;
            for (;;)
            {
                if (BepInEx.UnityInput.Current.GetKey(KeyCode.F1))
{
		Log("stopped !!");
		 break;
	}
			Character.GainFullStamina();
	view.RPC("RPCA_Revive", Photon.Pun.RpcTarget.All, true);
                while (c.data.isClimbingAnything)
                {
                    c.refs.climbing.playerSlide += Vector2.up * 30f;
                    await System.Threading.Tasks.Task.Delay(10);
                }
                await System.Threading.Tasks.Task.Delay(1);
            }
        */
        public override void Method()
        {
            var local = CheatPlayer.LocalPlayer;
            Character.GainFullStamina();
            foreach (var effect in Enum.GetValues(typeof(CharacterAfflictions.STATUSTYPE)))
                local.GameCharacter.refs.afflictions.SetStatus((CharacterAfflictions.STATUSTYPE)effect, 0f);
        }
    }
}