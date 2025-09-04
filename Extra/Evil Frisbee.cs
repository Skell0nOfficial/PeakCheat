using HarmonyLib;
using PeakCheat.Types;
using Photon.Pun;
using UnityEngine;
using FS = PeakCheat.Extra.EvilFrisbee.FrisbeeScript;

namespace PeakCheat.Extra
{
    [HarmonyPatch]
    internal class EvilFrisbee: UITab, CheatBehaviour
    {
        public class FrisbeeScript: MonoBehaviour
        {
            static FS? Instance
            {
                get
                {
                    if (!PhotonNetwork.InRoom) return null;
                    if (GameObject.FindFirstObjectByType<FS>()?.gameObject is GameObject obj && obj.TryGetComponent<FS>(out FS script)) return script;

                    return new GameObject("Frisbee").AddComponent<FS>();
                }
            }
        }
        public override string Name => "Evil Disk";
        public override int Order => 5;
        public override void Render()
        {
            GUILayout.Label("The evil disk of doom >:)");
        }
    }
}