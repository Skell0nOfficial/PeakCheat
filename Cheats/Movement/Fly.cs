using PeakCheat.Types;
using UnityEngine;
using Zorro.Core;
using PeakCheat.Patches;
using PeakCheat.Utilities;
using PeakCheat.Main;

namespace PeakCheat.Cheats.Movement
{
    internal class Fly: Cheat
    {
        public override string Name => "Fly";
        public override string Description => "Lets you fly around";
        public override SceneType RequiredScene => SceneType.Airport;
        private static bool _enabled = false;
        public override void Enable() => Patch(_enabled);
        public override void Method()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                AudioUtil.Click();
                _enabled = !_enabled;
                Patch(_enabled);
            }

            if (!_enabled) return;

            var input = CharacterInput.action_move.ReadValue<Vector2>() * 25f;
            var transform = Singleton<MainCameraMovement>.Instance.transform;
            var vector = transform.TransformDirection(new Vector3(input.x, Input.GetKey(KeyCode.E)? (Input.GetKey(KeyCode.Q)? 0f: 50f): (Input.GetKey(KeyCode.Q) ? -50f : 0f), input.y));
            var character = Character.localCharacter;
            var dat = character.data;
            var ragdoll = character.refs.ragdoll;

            if (!UIHandler.Open)
            {
                character.input.SendMessage("Sample", true);
                character.refs.movement.Invoke("CameraLook", 0f);
            }

            dat.fallSeconds = 0f;
            dat.passedOut = false;
            dat.passOutValue = 0f;
            dat.sinceGrounded = -.1f;
            dat.fullyPassedOut = false;
            dat.passedOutOnTheBeach = 0f;

            ragdoll.HaltBodyVelocity();
            ragdoll.MoveAllRigsInDirection(vector.normalized);
        }
        public override void Disable() => Patch(false);
        private static void Patch(bool enabled)
        {
            MovementPatch.Freeze(enabled);
            GravityPatch.Gravity(!enabled);
        }
    }
}