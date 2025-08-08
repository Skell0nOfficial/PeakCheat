using ExitGames.Client.Photon;
using PeakCheat.Classes;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;

namespace PeakCheat.Utilities
{
    internal class ACDisabler: MonoBehaviourPunCallbacks, EventBehaviour
    {
        private static readonly List<int> _anticheatPlayers = new List<int>();
        private string Name => GetType().Name;
        private static readonly string[] _blacklistedProps = new string[]
        {
            "AtlUser",
            "AtlOwner",
            "CherryUser",
            "CherryOwner"
        };
        public static bool UsingAntiCheat(CheatPlayer player) => _anticheatPlayers.Contains(player.PhotonPlayer?.ActorNumber?? -1);
        void EventBehaviour.OnEvent(byte code, int sender, object data)
        {
            if (!PhotonNetwork.InRoom) return;

            var player = PhotonNetwork.CurrentRoom.GetPlayer(sender);
            if (code == 69 && player != null)
            {
                _anticheatPlayers.Add(sender);
                PlayerUtil.BreakGame(player);
            }
        }
        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (_blacklistedProps.Any(str => changedProps.ContainsKey(str)))
                PlayerUtil.BreakGame(targetPlayer);
        }
    }
}
