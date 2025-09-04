using PeakCheat.Main;
using PeakCheat.Utilities;
using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts.Extensions;
using UnityEngine;

namespace PeakCheat.Types
{
    internal class JoinRandom: MonoBehaviourPunCallbacks
    {
        public static void Join() => Plugin.Instance?.gameObject.GetOrAddComponent<JoinRandom>().StartProcess();
        public void StartProcess()
        {
            if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
            if (PhotonNetwork.Server != ServerConnection.MasterServer)
            {
                PhotonNetwork.Disconnect();
                return;
            }

            PhotonNetwork.JoinLobby();
        }
        public override void OnDisconnected(DisconnectCause cause) => PhotonNetwork.ConnectUsingSettings();
        public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();
        public override void OnJoinedLobby() => PhotonNetwork.JoinRandomRoom();
        public override void OnJoinedRoom()
        {
            for (int i = 0; i < 3; i++) GeneralUtil.DelayInvoke(() => AudioUtil.Click(), (i + 1) * .5f);
            GUIUtility.systemCopyBuffer = PhotonNetwork.CurrentRoom.Name;
            PhotonNetwork.Disconnect();
            GameObject.Destroy(this);
        }
    }
}