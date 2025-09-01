using PeakCheat.Types;
using PeakCheat.Utilities;
using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PeakCheat.Cheats.Visuals
{
    internal class ESP: Cheat
    {
        public override string Name => "ESP";
        public override string Description => "Lets you see players through walls";
        private static Material GUIMaterial = new Material(Shader.Find("GUI/Text Shader"));
        private static readonly BodypartType[] _parts = new BodypartType[]
        {
            // Start
            BodypartType.Head,
            //Left Arm
            BodypartType.Arm_L,
            BodypartType.Elbow_L,
            BodypartType.Hand_L,
            // Reverse Left Arm
            BodypartType.Elbow_L,
            BodypartType.Arm_L,
            BodypartType.Head,
            // Right Arm
            BodypartType.Arm_R,
            BodypartType.Elbow_R,
            BodypartType.Hand_R,
            // Reverse Right Arm
            BodypartType.Elbow_R,
            BodypartType.Arm_R,
            BodypartType.Head,
            // Down
            BodypartType.Torso,
            BodypartType.Hip,
            // Left Leg
            BodypartType.Knee_L,
            BodypartType.Foot_L,
            // Reverse Left Leg
            BodypartType.Knee_L,
            BodypartType.Foot_L,
            BodypartType.Hip,
            // Left Leg
            BodypartType.Knee_R,
            BodypartType.Foot_R,
            // Reverse Left Leg
            BodypartType.Knee_R,
            BodypartType.Foot_R,
            BodypartType.Hip
        };
        public override void Method()
        {
            foreach (var player in PlayerUtil.OtherPlayers())
            {
                var character = player.GameCharacter;
                if (character == null || character.data.dead) continue;

                var line = character.gameObject.GetOrAddComponent<LineRenderer>();
                if (!(character.refs?.ragdoll?.partDict is Dictionary<BodypartType, Bodypart> dict)) return;

                var positions = _parts
                    .Select(P => dict.Where(K => _parts.Contains(P)).First(B => B.Key == P))
                    .Select(B => B.Value?.WorldCenterOfMass()?? character.Center)
                    .ToArray();

                if (line.material == null || line.material != GUIMaterial) line.material = GUIMaterial;

                line.startWidth = .07f;
                line.endWidth = line.startWidth;
                line.startColor = player.PlayerColor;
                line.endColor = player.PlayerColor;
                line.positionCount = positions.Length;
                line.SetPositions(positions);
            }
        }
        public override void Disable()
        {
            foreach (var character in Character.AllCharacters)
            {
                if (character == null || character.IsLocal) continue;
                if (character.TryGetComponent<LineRenderer>(out var line))
                {
                    line.enabled = false;
                    line.material.color = Color.clear;
                    GameObject.Destroy(line, .1f);
                }
            }
        }
    }
}