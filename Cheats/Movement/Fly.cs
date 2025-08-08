using PeakCheat.Classes;
using PeakCheat.Patches;
using UnityEngine;
using Zorro.Core;

namespace PeakCheat.Cheats.Movement
{
    internal class Fly: Cheat
    {
        public override string Description => "Lets you fly around";
        private static Vector3 _gravity = Vector3.zero;
        public override void Enable()
        {
            MovementPatch.Freeze(true);
            _gravity = Physics.gravity;
            Physics.gravity = Vector3.zero;
        }
        public override void Method()
        {
            var input = CharacterInput.action_move.ReadValue<Vector2>() * 25f;
            var transform = Singleton<MainCameraMovement>.Instance.transform;
            var vector = transform.TransformDirection(new Vector3(input.x, CharacterInput.action_scroll.ReadValue<float>() * 50f, input.y));

            Character.localCharacter.refs.movement.Invoke("CameraLook", float.Epsilon);
            foreach (var part in Character.localCharacter.refs.ragdoll.partList)
                part.Rig.linearVelocity = vector;
        }
        public override void Disable()
        {
            MovementPatch.Freeze(false);
            Physics.gravity = _gravity;
        }
    }
}