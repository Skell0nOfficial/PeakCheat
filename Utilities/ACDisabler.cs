using ExitGames.Client.Photon;
using PeakCheat.Patches;
using PeakCheat.Types;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;

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
        private const string ActorModifierID = "TargetActorModifier - ACDisabler";
        bool CheatBehaviour.DelayStart() => true;
        void CheatBehaviour.Start()
        {
            PhotonCallbacks.PropertiesUpdate += CheatCheck;
            OperationPatch.AddEventPatch(200, GetParams, ActorModifierID);
        }
        private static ParameterDictionary GetParams(ParameterDictionary dict)
        {
            var newDict = new ParameterDictionary();

            foreach (var key in dict.paramDict.Keys) newDict[key] = dict[key];

            foreach (var param in dict)
            {
                if (param.Key is byte b)
                {
                    if (b == 245 && param.Value is Hashtable RPCData)
                        if (RPCData.TryGetValue(3, out var text) && text is string RPCName)
                        {
                            if (RPCName == "RPC_Explode" || RPCName == "CreatePrefabRPC") continue;
                            newDict[253] = PhotonNetwork.PlayerList
                                .Select(I => I.ActorNumber)
                                .Where(I => !_anticheatPlayers.Contains(I))
                                .ToArray();
                            newDict[246] = (byte)ReceiverGroup.All;
                            continue;
                        }
                    if (b == 246) continue;
                }
            }

            return newDict;
        }
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
                if (props.TryGetValue(str, out var val) && val is object obj && obj.ToString() != ForcedPropertyKey) return true;
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