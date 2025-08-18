using PeakCheat.Types;
using Photon.Pun;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public class PrefabUtil: CheatBehaviour
    {
        public static void SummonExplosion(Vector3 position) => PhotonNetwork.InstantiateItem("Dynamite", position, Quaternion.identity).GetPhotonView().RPC("RPC_Explode", RpcTarget.All);
    }
}