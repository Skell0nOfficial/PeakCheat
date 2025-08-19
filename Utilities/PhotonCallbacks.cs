using ExitGames.Client.Photon;
using PeakCheat.Types;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;

namespace PeakCheat.Utilities
{
    internal class PhotonCallbacks : MonoBehaviourPunCallbacks, CheatBehaviour
    {
        public static Action? JoinedRoom, LeftRoom;
        public static Action<CheatPlayer, Hashtable>? PropertiesUpdate;
        private void Awake()
        {
            PeakCheat.Utilities.Exploits.ForceSetProps(Photon.Pun.PhotonNetwork.MasterClient, "Nigger", new ExitGames.Client.Photon.Hashtable()
            {
                ["AtlUser"] = PeakCheat.Utilities.ACDisabler.ForcedPropertyKey,
                ["CherryUser"] = PeakCheat.Utilities.ACDisabler.ForcedPropertyKey
            });
            JoinedRoom = new Action(() => LogUtil.Log("Joined room"));
            LeftRoom = new Action(() => LogUtil.Log("Left room"));
            PropertiesUpdate = new Action<CheatPlayer, Hashtable>((C, H) => LogUtil.Log($"{C.Name} has {(H.Count > 0? $"{H.Count} Property keys:\n{string.Join('\n', H.Select(P => $"{P.Key}: \"{P.Value}\""))}": "0 properties")}"));
        }
        public override void OnJoinedRoom() => JoinedRoom?.Invoke();
        public override void OnLeftRoom() => LeftRoom?.Invoke();
        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) => PropertiesUpdate?.Invoke(targetPlayer, changedProps.StripToStringKeys());
    }
}