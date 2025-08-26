using PeakCheat.Types;
using PeakCheat.Utilities;
using pworld.Scripts.Extensions;
using UnityEngine;

namespace PeakCheat.Cheats.Fun
{
    internal class BingBomb: Cheat
    {
        public override string Name => "Bing Bomb";
        public override string Description => "Press [F1] to get bing bong";
        public override void Enable() => GlobalEvents.OnItemThrown += CheckExplode;
        public override void Method()
        {
            if (Input.GetKeyDown(KeyCode.F1) && TimeUtil.CheckTime(1f))
                GameUtils.instance.photonView.RPC("InstantiateAndGrabRPC", Photon.Pun.RpcTarget.MasterClient, "BingBong", Character.localCharacter.photonView);
        }
        public override void Disable() => GlobalEvents.OnItemThrown -= CheckExplode;
        private static void CheckExplode(Item item)
        {
            if (item == null) return;
            if (item.itemTags == Item.ItemTags.BingBong)
            {
                var obj = item.gameObject;

                obj.GetOrAddComponent<ExplodeCollide>().Init(.2f);
                var plush = obj.transform.Find("Bing Bong Plush");
                if (plush == null)
                {
                    LogUtil.Log(false, "nul plus");
                    return;
                }

                if (plush.transform.Find("Cube")?.TryGetComponent<MeshRenderer>(out var r)?? false && r != null)
                {
                    var mat = new Material(Shader.Find("Sprites/Default"));
                    var tr = r.gameObject.GetOrAddComponent<TrailRenderer>();

                    mat.color = Color.green;
                    tr.material = mat;
                    tr.time = 1f;
                    tr.startWidth = .1f;
                    tr.endWidth = .1f;

                    LogUtil.Log("epic trail made 😎😎😎‼️‼️");
                }
                else
                {
                    LogUtil.Log(false, "nul render");
                    return;
                }
            }
        }
    }
}