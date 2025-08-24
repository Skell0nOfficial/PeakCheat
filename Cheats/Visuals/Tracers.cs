using PeakCheat.Types;
using PeakCheat.Utilities;
using UnityEngine;

namespace PeakCheat.Cheats.Visuals
{
    internal class Tracers: Cheat
    {
        public override string Name => "Tracers";
        public override string Description => "Renders lines from your current position to every player";
        private static Material GUIMaterial = new Material(Shader.Find("GUI/Text Shader"));
        public override void Method()
        {
            foreach (var player in PlayerUtil.OtherPlayers())
            {
                var line = new GameObject($"LineObject:{Time.time * 5f}:{player.Name}").AddComponent<LineRenderer>();
                var start = UnityUtil.CurrentPosition() + (Vector3.down * 4f);

                line.material = GUIMaterial;
                line.startColor = player.PlayerColor;
                line.endColor = player.PlayerColor;
                line.startWidth = .1f;
                line.endWidth = .1f;
                line.SetPositions(new Vector3[]
                {
                    start,
                    player.BodyTransform?.position?? Vector3.zero
                });

                GameObject.Destroy(line.gameObject, 1.1f);
                GameObject.Destroy(line, Mathf.Max(.1f, Time.deltaTime));
            }
        }
    }
}