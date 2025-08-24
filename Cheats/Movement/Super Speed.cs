using PeakCheat.Types;
using System.Linq;
using UnityEngine;

namespace PeakCheat.Cheats.Movement
{
    public class SuperSpeed: Cheat
    {
        public override string Name => "Speed Modifier";
        public override string Description => "Adjusts your speed depending on your scroller";
        private static float _boost = 1f;
        public override void Method()
        {
            GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(1).customizationData.currentHat.ToString();
            _boost += CharacterInput.action_scroll.ReadValue<float>() * .3f;
            _boost = Mathf.Max(1f, _boost);
            if (Input.GetKeyDown(KeyCode.RightShift)) _boost = 1f;
            var movement = Character.localCharacter.refs.movement;
            movement.movementModifier = _boost;
            movement.airMovementTurnSpeed = _boost;
        }
        public override void Disable() => Character.localCharacter.refs.movement.movementModifier = 1f;
    }
}