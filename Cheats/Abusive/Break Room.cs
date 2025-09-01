using PeakCheat.Types;
using PeakCheat.Utilities;
using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;
using Zorro.UI.Modal;

namespace PeakCheat.Cheats.Abusive
{
    internal class BreakRoom: Cheat
    {
        public override string Name => "Break Room";
        public override string Description => "Encrypts every room operation, causing nothing to run on the server";
        public override SceneType RequiredScene => SceneType.Airport;
        public override async void Enable()
        {
            if (RunManager.Instance?.photonView is PhotonView run) run.ForceDestroy();
            if (GameUtils.instance?.photonView is PhotonView game) game.ForceDestroy();
            if (GameOverHandler.Instance?.view is PhotonView game2) game2.ForceDestroy();
            if (OrbFogHandler.Instance?.photonView is PhotonView orbFog) orbFog.ForceDestroy();
            if (DayNightManager.instance?.photonView is PhotonView time) time.ForceDestroy();
            if (FakeItemManager.Instance?.photonView is PhotonView items) items.ForceDestroy();
            if (ReconnectHandler.Instance?.photonView is PhotonView reconnect) reconnect.ForceDestroy();
            if (Object.FindFirstObjectByType<PeakSequence>()?.view is PhotonView sequence) sequence.ForceDestroy();
            if (Object.FindFirstObjectByType<CharacterSpawner>()?.photonView is PhotonView spawner) spawner.ForceDestroy();
            if (Object.FindFirstObjectByType<AirportCheckInKiosk>()?.photonView is PhotonView kiosk) kiosk.ForceDestroy();
            Enabled = false;
            
            await Task.Delay(2000);
            Modal.OpenModal(new DefaultHeaderModalOption("PeakCheat".WithColor((Color.white * .4f).WithAlpha(1f)), "This room is now broken, you sure you wanna stay in it?"), new ModalButtonsOption(new ModalButtonsOption.Option[]
            {
                new ModalButtonsOption.Option("Stay", () => {}),
                new ModalButtonsOption.Option("Leave", () => PhotonNetwork.LeaveRoom(true)),
            }));
        }
    }
}