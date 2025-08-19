using PeakCheat.Types;
using PeakCheat.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PeakCheat.Extra
{
    internal class Simulators: UITab, CheatBehaviour
    {
        void CrashOthers()
        {
            if (Photon.Pun.PhotonNetwork.InstantiateItem("Dynamite", Vector3.one * (float.MaxValue / 23.45982349873548325798235F), Quaternion.identity).TryGetComponent<Photon.Pun.PhotonView>(out var v))
                for (int i = 0; i < 1500; i++)
                    PeakCheat.Utilities.GeneralUtil.DelayInvoke(() => v.RPC("RPC_Explode", PlayerHandler.GetAllPlayerCharacters().Find(C => C.characterName.Contains("")).photonView.Owner), .2f);
        }
        void saved()
        {
            var p = PlayerHandler.GetAllPlayerCharacters().Find(C => C.characterName.Contains("Gae"));
            var c = MainCameraMovement.specCharacter;
            var l = Character.localCharacter;

            PeakCheat.Utilities.PlayerUtil.Faint(p);
        }
        void TP()
        {
            int num = 1;

            foreach (var c in PlayerHandler.GetAllPlayerCharacters())
            {
                if (c.IsLocal) continue;
                if (c.data.dead) PeakCheat.Utilities.PlayerUtil.Revive(c);

                PeakCheat.Utilities.LogUtil.Log($"Teleporting {c.characterName.Replace(" ", "")}..");
                PeakCheat.Utilities.PlayerUtil.Teleport(c, Character.localCharacter.Head + (Vector3.up * num++));
                PeakCheat.Utilities.PlayerUtil.PlayerRPC(c, "MoraleBoost", 100f, 0);
            }
        }
        public override string Name => "Simulators";
        public static void BingBongPP()
        {
            var penisOffsets = new List<UnityEngine.Vector3>();
            var penisWidth = 10;
            for (int i = 0; i < penisWidth; i++) penisOffsets.Add(UnityEngine.Vector3.right * (i + 1));
            for (int i = 0; i < (penisWidth * 1.8f); i++) penisOffsets.Add(UnityEngine.Vector3.right * ((penisWidth / 2f) + .5f) + (UnityEngine.Vector3.up * i));
            var t = UnityEngine.Camera.main.transform;
            var curPos = t.position;
            foreach (var pos in penisOffsets)
                Photon.Pun.PhotonNetwork
                    .InstantiateItem(
                    (UnityEngine.Mathf.Abs(penisOffsets.IndexOf(pos) - penisOffsets.Count) < 6)
                    ? "Lollipop" : "BingBong",
                    UnityEngine.Vector3.zero,
                    UnityEngine.Quaternion.identity)
                    .GetComponent<Photon.Pun.PhotonView>()
                    .RPC("SetKinematicRPC", Photon.Pun.RpcTarget.All,
                    true, curPos + (t.forward * 15f)
                    + (pos - (UnityEngine.Vector3.right * (penisWidth / 2f))),
                    UnityEngine.Quaternion.identity);
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
            var names = Resources.FindObjectsOfTypeAll<Item>().Select(I => I.name.Replace("(Clone)", "")).ToArray();
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