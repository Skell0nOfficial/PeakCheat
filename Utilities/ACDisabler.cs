using ExitGames.Client.Photon;
using PeakCheat.Types;
using Photon.Pun;
using System.Collections.Generic;

namespace PeakCheat.Utilities
{
    internal class ACDisabler: EventBehaviour, CheatBehaviour
    {
        private static readonly List<int> _anticheatPlayers = new List<int>(), _cheaters = new List<int>();
        private static readonly string[] _blacklistedProps = new string[]
        {
            "AtlUser",
            "AtlOwner",
            "CherryUser",
            "CherryOwner"
        };
        public const string ForcedPropertyKey = "PeakCheat::ForcedProperty";
        bool CheatBehaviour.DelayStart() => true;
        void CheatBehaviour.Start() => PhotonCallbacks.PropertiesUpdate += CheatCheck;
        public static bool UsingAntiCheat(CheatPlayer player) => _anticheatPlayers.Contains(player.PhotonPlayer?.ActorNumber?? -1);
        void EventBehaviour.OnEvent(byte code, int sender, object data)
        {
            if (!PhotonNetwork.InRoom) return;

            if (code == 69 && PhotonNetwork.TryGetPlayer(sender, out var player))
            {
                LogUtil.Log($"Found Anticheat User: {player.NickName}");
                _anticheatPlayers.AddIfNew(sender);
            }
        }
        private void CheatCheck(CheatPlayer player, Hashtable props)
        {
            if (_blacklistedProps.Any(str =>
            {
                if (props.TryGetValue(str, out var val) &&
                val is string value &&
                value != ForcedPropertyKey)
                    return true;
                return false;
            }, out var val))
            {
                int actor = player.PhotonPlayer?.ActorNumber ?? 0;
                if (!_cheaters.Contains(actor))
                {
                    player.Crash();
                    LogUtil.Log($"Found [{(val.Contains("Atl")? "Atlas": "Cherry")}] User: {player.Name}");
                }
                if (actor != 0) _cheaters.AddIfNew(actor);
            }
        }
    }
}