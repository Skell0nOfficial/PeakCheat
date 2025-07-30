using PeakCheat.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PeakCheat.Utilities
{
    internal class TeleportUtil: CheatBehaviour
    {
        private static Queue<KeyValuePair<CheatPlayer, Vector3>> _teleports = new Queue<KeyValuePair<CheatPlayer, Vector3>>();
        public override void Start() => ProcessTeleportation();
        public static void Teleport(CheatPlayer player, Vector3 pos) => _teleports.Enqueue(new KeyValuePair<CheatPlayer, Vector3>(player, pos));
        private static async void ProcessTeleportation()
        {
            while (true)
            {
                if (_teleports.TryDequeue(out var pair))
                {
                    var player = pair.Key;
                    var pos = pair.Value;
                    player.Teleport(pos, true);
                    await Task.Delay(750);
                }
                await Task.Delay(1);
            }
        }
    }
}
