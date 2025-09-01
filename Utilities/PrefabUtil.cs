using PeakCheat.Types;
using Photon.Pun;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public class PrefabUtil: CheatBehaviour
    {
        public static void SummonExplosion(Vector3 position)
        {
            var view = PhotonNetwork.InstantiateItem("Dynamite", position, Quaternion.identity).GetPhotonView();
            view.RPC("RPC_Explode", RpcTarget.All);
            GeneralUtil.DelayInvoke(() => PhotonNetwork.Destroy(view), .2f);
        }
    }
}