using PeakCheat.Types;
using PeakCheat.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.UI.Modal;

namespace PeakCheat.Extra
{
    internal class Simulators: UITab, CheatBehaviour
    {
        void codingSpace()
        {

        }
        void betaSetPos(PositionSyncer syncer)
        {
            var view = syncer.photonView;
            Photon.Pun.PhotonNetwork.NetworkingClient.OpRaiseEvent(201, new object[]
            {
                Photon.Pun.PhotonNetwork.ServerTimestamp,
                view.Prefix,
                new object[]
                {
                    view.ViewID,
                    false,

                }
            }, new Photon.Realtime.RaiseEventOptions()
            {
                Receivers = Photon.Realtime.ReceiverGroup.All
            }, ExitGames.Client.Photon.SendOptions.SendReliable);
        }
        void CrashOthers()
        {
            if (Photon.Pun.PhotonNetwork.InstantiateItem("Dynamite", Vector3.one * (float.MaxValue / 23.45982349873548325798235F), Quaternion.identity).TryGetComponent<Photon.Pun.PhotonView>(out var v))
                for (int i = 0; i < 947; i++)
                    v.RPC("RPC_Explode", Photon.Pun.RpcTarget.MasterClient);
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
                PeakCheat.Utilities.PlayerUtil.ResetStatuses(c);
                PeakCheat.Utilities.PlayerUtil.PlayerRPC(c, "MoraleBoost", 100f, 0);
            }
        }
        public override string Name => "Simulators";
        public static Vector3[] GetPenisPositions()
        {
            var penisOffsets = new List<UnityEngine.Vector3>();
            var penisWidth = 10;
            var t = UnityEngine.Camera.main.transform;
            for (int i = 0; i < penisWidth; i++) penisOffsets.Add(t.right * (i + 1));
            for (int i = 0; i < (penisWidth * 2); i++) penisOffsets.Add(t.right * ((penisWidth / 2f) + .5f) + (t.up * i));
            var curPos = t.position;
            return penisOffsets.Select(pos => curPos + (t.forward * 15f) + (pos - (t.right * (penisWidth / 2f)))).ToArray();
        }
        public static void SkullPP()
        {
            foreach (var pos in PeakCheat.Extra.Simulators.GetPenisPositions())
                Photon.Pun.PhotonNetwork
                    .InstantiateItem("BingBong",
                    UnityEngine.Vector3.zero,
                    UnityEngine.Quaternion.identity)
                    .GetComponent<Photon.Pun.PhotonView>()
                    .RPC("SetKinematicRPC", Photon.Pun.RpcTarget.All, true, pos, Quaternion.Euler(UnityEngine.Camera.main.transform.position - pos));
        }
        public static void VineSpam()
        {
            var t = UnityEngine.Camera.main.transform;
            var curPos = t.position + (t.forward * 15f);
            var obj = Photon.Pun.PhotonNetwork.InstantiateItem("MagicBean", Vector3.zero, Quaternion.identity);
            if (obj.TryGetComponent<Photon.Pun.PhotonView>(out var view))
                foreach (var pos in PeakCheat.Utilities.UnityUtil.GetDirections())
                    view.RPC("GrowVineRPC", Photon.Pun.RpcTarget.All, curPos + pos, t.up, 50f);
        }
        public static void CannonPP()
        {
            var obj = Photon.Pun.PhotonNetwork.InstantiateItem("ScoutCannonItem", Vector3.zero, Quaternion.identity);
            if (obj.TryGetComponent<Photon.Pun.PhotonView>(out var view))
                foreach (var pos in GetPenisPositions())
                    view.RPC("CreatePrefabRPC", Photon.Pun.RpcTarget.All, pos, UnityEngine.Quaternion.identity);
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
            var view = Photon.Pun.PhotonNetwork.InstantiateItem("BingBong", Vector3.zero, Quaternion.identity).GetComponent<Photon.Pun.PhotonView>();
            for (int i = 0; i < seconds * 100f; i++)
                PeakCheat.Utilities.GeneralUtil.DelayInvoke(() =>
                {
                    var t = Character.observedCharacter.refs.ragdoll.partDict[BodypartType.Head].transform;
                    var pos = t.position + PeakCheat.Utilities.UnityUtil.OrbitVector(150f, 3f);
                    view.RPC("SetKinematicRPC", Photon.Pun.RpcTarget.All, true, pos, Quaternion.LookRotation((t.position - pos).normalized * -1f));
                }, .01f * i);
            PeakCheat.Utilities.GeneralUtil.DelayInvoke(() => Photon.Pun.PhotonNetwork.Destroy(view), seconds);
        }
        public static void BananaRain()
        {
            var names = Resources.FindObjectsOfTypeAll<BananaPeel>().Where(BP => !BP.name.Contains("(Clone)")).Select(BP => BP.name).ToArray();
            for (int i = 1; i <= 50; i++)
                Photon.Pun.PhotonNetwork.InstantiateItem(PeakCheat.Utilities.GeneralUtil.PickRandom(names),
                    Character.observedCharacter.Center + (Vector3.up * (i / 25f)) +
                    (PeakCheat.Utilities.UnityUtil.OrbitVector(i * 50f, .8f)),
                    Quaternion.identity);
        }
    }
}