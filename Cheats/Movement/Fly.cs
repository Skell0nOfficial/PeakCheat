using PeakCheat.Types;
using PeakCheat.Main;
using PeakCheat.Patches;
using UnityEngine;
using Zorro.Core;

namespace PeakCheat.Cheats.Movement
{
    internal class Fly: Cheat
    {
        public override string Name => "Fly";
        public override string Description => "Lets you fly around";
        public override SceneType RequiredScene => SceneType.Airport;
        public override void Enable()
        {
            MovementPatch.Freeze(true);
            GravityPatch.Gravity(false);
        }
        public override void Method()
        {
            var input = CharacterInput.action_move.ReadValue<Vector2>() * 25f;
            var transform = Singleton<MainCameraMovement>.Instance.transform;
            var vector = transform.TransformDirection(new Vector3(input.x, Input.GetKey(KeyCode.E)? (Input.GetKey(KeyCode.Q)? 0f: 50f): (Input.GetKey(KeyCode.Q) ? -50f : 0f), input.y));
            var character = Character.localCharacter;

            if (!UIHandler.Open)
            {
                character.input.SendMessage("Sample", true);
                character.refs.movement.Invoke("CameraLook", float.Epsilon);
            }

            character.data.sinceGrounded = -.1f;

            foreach (var part in character.refs.ragdoll.partList) part.Rig.linearVelocity = vector * (Input.GetKey(KeyCode.LeftShift)? 5f: 1f);
        }
        public override void Disable()
        {
            MovementPatch.Freeze(false);
            GravityPatch.Gravity(true);
        }
    }
}