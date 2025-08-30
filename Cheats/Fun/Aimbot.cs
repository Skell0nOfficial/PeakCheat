using PeakCheat.Types;
using PeakCheat.Utilities;
using Photon.Pun;
using UnityEngine;

namespace PeakCheat.Cheats.Fun
{
    internal class Aimbot: Cheat
    {
        public override string Name => "Grapple Aimbot";
        public override string Description => "Gives your grappling hook aimbot when you right click";
        public override SceneType RequiredScene => SceneType.Airport;
        public override void Method()
        {
            if (Input.GetMouseButtonDown(1))
            {
                var player = CheatPlayer.LocalPlayer;
                if (player.GetItem(out var i) && i is Item item && item.TryGetComponent<RescueHook>(out var hook) && hook is RescueHook grapple)
                {
                    var dir = Camera.main.ScreenPointToRay(Input.mousePosition).direction.normalized;
                    var dis = float.MaxValue;
                    var players = PlayerUtil.OtherPlayers();
                    Character? closestPlayer = null;

                    foreach (var p in players)
                    {
                        var playerDis = Vector3.Distance((p.Center - player.Center).normalized, dir);

                        if (playerDis < dis)
                        {
                            dis = playerDis;
                            closestPlayer = p;
                        }
                    }

                    if (closestPlayer is Character character)
                    {
                        grapple.photonView.RPC("RPCA_RescueCharacter", RpcTarget.All, character.photonView);
                        if (character.ToCheatPlayer() is var C) C.SetVelocity(Vector3.up * 5f);
                    }
                }
            }
        }
    }
}