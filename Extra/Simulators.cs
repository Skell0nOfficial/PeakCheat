using PeakCheat.Types;
using PeakCheat.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PeakCheat.Extra
{
    internal class Simulators: UITab, CheatBehaviour
    {
        void saved()
        {
            var p = PlayerHandler.GetAllPlayerCharacters().Find(C => C.characterName.Contains(""));
            var c = MainCameraMovement.specCharacter;
            var l = Character.localCharacter;
            PeakCheat.Utilities.PlayerUtil.Crash(c);
        }
        public override string Name => "Simulators";
        public static void BingBongPP()
        {
            List <Vector3> penisOffsets = new List<Vector3>();
            int penisWidth = 10;
            for (int i = 0; i < penisWidth; i++) penisOffsets.Add(Vector3.right * (i + 1));
            for (int i = 0; i < (penisWidth * 1.8f); i++)
                penisOffsets.Add(Vector3.right * ((penisWidth / 2f) + .5f) + (Vector3.up * i));
            var t = Camera.main.transform;
            var curPos = t.position;
            foreach (var pos in penisOffsets)
            {
                var view = Photon.Pun.PhotonNetwork.InstantiateItem((Mathf.Abs(Array.IndexOf(penisOffsets.ToArray(), pos)
                    - penisOffsets.Count) < 6) ? "Lollipop" :
                    "BingBong", Vector3.zero, Quaternion.identity)
                    .GetComponent<Photon.Pun.PhotonView>();
                view.RPC("SetKinematicRPC", Photon.Pun.RpcTarget.All, true, curPos + (t.forward * 15f) + (pos - (Vector3.right * (penisWidth / 2f))), Quaternion.identity);
            }
        }
        public static void BingBongWave()
        {
            var startPos = Camera.main.transform.position + (Vector3.up * 5f);
            var views = new List<Photon.Pun.PhotonView>();

            for (int i = 0; i < 100; i++) views.Add(Photon.Pun.PhotonNetwork.InstantiateItem("BingBong", Vector3.zero, Quaternion.identity).GetComponent<Photon.Pun.PhotonView>());
            float seconds = 20f;

            for (int i = 0; i < seconds * 2f; i++)
            {
                PeakCheat.Utilities.GeneralUtil.DelayInvoke(() =>
                {
                    var current = Mathf.Clamp((int)UnityEngine.Mathf.PingPong(UnityEngine.Time.time * 5f, (views.Count / 10f)) * (views.Count / 10f), views.Count / 10f, views.Count);
                    foreach (var view in views)
                    {
                        int index = Array.IndexOf(views.ToArray(), view);
                        var pos = startPos + (Vector3.right * (index + 1)) + (Vector3.up * Mathf.Lerp(views.Count / 5f, 0f, Mathf.Abs(index - current) / (views.Count / 2)));
                        view.RPC("SetKinematicRPC", Photon.Pun.RpcTarget.All, true, pos, Quaternion.LookRotation((Camera.main.transform.position - pos).normalized * -1f));
                    }
                }, .5f * i);
            }

            PeakCheat.Utilities.GeneralUtil.DelayInvoke(() =>
            {
                foreach (var view in views)
                    Photon.Pun.PhotonNetwork.Destroy(view);
            }, seconds + (.1f));
        }
        public static void BingBongOrbit()
        {
            float seconds = 10f;
            if (!TimeUtil.CheckTime(seconds)) return;
            var t = Camera.main.transform;
            var view = Photon.Pun.PhotonNetwork.InstantiateItem("BingBong", Vector3.zero, Quaternion.identity).GetComponent<Photon.Pun.PhotonView>();
            for (int i = 0; i < seconds * 100f; i++)
                PeakCheat.Utilities.GeneralUtil.DelayInvoke(() =>
                {
                    var pos = t.position + PeakCheat.Utilities.UnityUtil.OrbitVector(150f, 3f);
                    view.RPC("SetKinematicRPC", Photon.Pun.RpcTarget.All, true, pos, Quaternion.LookRotation((t.position - pos).normalized * -1f));
                }, .01f * i);
            PeakCheat.Utilities.GeneralUtil.DelayInvoke(() => Photon.Pun.PhotonNetwork.Destroy(view), seconds);
        }
        public static void ItemRain()
        {
            var names = new string[] { "BingBong" };
            for (int i = 0; i < 200; i++)
            {
                var view = Photon.Pun.PhotonNetwork.InstantiateItem(PeakCheat.Utilities.GeneralUtil.PickRandom(names),
                    Camera.main.transform.position + (Vector3.up * (5f + (i + 1))) +
                    (PeakCheat.Utilities.UnityUtil.OrbitVector(i + 1 * 47f, 5f)),
                    Quaternion.identity)
                    .GetComponent<Photon.Pun.PhotonView>();
                PeakCheat.Utilities.GeneralUtil.DelayInvoke(() => Photon.Pun.PhotonNetwork.Destroy(view), 7f);
            }
        }
    }
}