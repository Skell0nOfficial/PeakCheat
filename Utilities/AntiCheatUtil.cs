using ExitGames.Client.Photon;
using PeakCheat.Classes;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;

namespace PeakCheat.Utilities
{
    internal class AntiCheatUtil : MonoBehaviourPunCallbacks, CheatBehaviour
    {
        private static bool _inRoom = false;
        public override void OnJoinedRoom()
        {
            _inRoom = true;
            GetMasterThenKick();
        }
        public override void OnLeftRoom() => _inRoom = false;
        private static async void GetMasterThenKick()
        {
            while (!PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                LogUtil.Log("[AntiCheatUtil] Sent Master Request");
                await Task.Delay(100);
            }
            if (PhotonNetwork.InRoom)
                SendKick();
        }
        private static async void SendKick()
        {
            LogUtil.Log("Sending kicks..");
            while (_inRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                    foreach (var player in PhotonNetwork.PlayerList)
                        PhotonNetwork.RaiseEvent(107,
                            ((object)player.ActorNumber).SingleArray(),
                        new RaiseEventOptions()
                        {
                            Receivers = ReceiverGroup.All
                        }, SendOptions.SendReliable);
                else if (_inRoom)
                {
                    LogUtil.Log("[AntiCheatUtil] Connected to room but not master?");
                    GetMasterThenKick();
                    break;
                }
                else break;
                await Task.Delay(100);
            }
            LogUtil.Log("[AntiCheatUtil] Stopped sending kicks");
        }
    }
}