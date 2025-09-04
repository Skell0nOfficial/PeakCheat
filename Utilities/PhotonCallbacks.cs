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
        public static Action<short, string>? JoinRoomFailed;
        public static Action<CheatPlayer, Hashtable>? PropertiesUpdate;
        private void Awake()
        {
            JoinedRoom = new Action(() => LogUtil.Log("Joined room"));
            LeftRoom = new Action(() => LogUtil.Log("Left room"));
            JoinRoomFailed = new Action<short, string>((C, M) => LogUtil.Log(false, $"Cant join room: {M} ({C})"));
            PropertiesUpdate = new Action<CheatPlayer, Hashtable>((C, H) => {});
        }
        public override void OnJoinedRoom() => JoinedRoom?.Invoke();
        public override void OnLeftRoom() => LeftRoom?.Invoke();
        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) => PropertiesUpdate?.Invoke(targetPlayer, changedProps.StripToStringKeys());
    }
}