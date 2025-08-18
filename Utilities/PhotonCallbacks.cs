using ExitGames.Client.Photon;
using PeakCheat.Types;
using Photon.Pun;
using System;

namespace PeakCheat.Utilities
{
    internal class PhotonCallbacks : MonoBehaviourPunCallbacks, CheatBehaviour
    {
        public static Action? JoinedRoom, LeftRoom;
        public static Action<CheatPlayer, Hashtable>? PropertiesUpdate;
        private void Awake()
        {
            JoinedRoom = new Action(() => LogUtil.Log("Joined room"));
            LeftRoom = new Action(() => LogUtil.Log("Left room"));
            PropertiesUpdate = new Action<CheatPlayer, Hashtable>((C, H) => {});
        }
    }
}