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
                item.gameObject.GetOrAddComponent<ExplodeCollide>().Init(.2f);
        }
    }
}