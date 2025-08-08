using HarmonyLib;
using PeakCheat.Classes;
using Photon.Pun;

namespace PeakCheat.Utilities
{
    [HarmonyPatch(typeof(PhotonNetwork), "OnEvent")]
    internal class EventPatch
    {
        public static void Postfix(ExitGames.Client.Photon.EventData photonEvent)
        {

            foreach (var eventCallback in Main.Main.TryGetBehaviours<EventBehaviour>())
                if (eventCallback != null)
                eventCallback.OnEvent(photonEvent.Code, photonEvent.Sender, photonEvent.CustomData);
        }
    }
}
