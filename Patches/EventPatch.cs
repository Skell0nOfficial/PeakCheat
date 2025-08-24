using HarmonyLib;
using PeakCheat.Main;
using PeakCheat.Types;
using Photon.Realtime;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(LoadBalancingClient), "OnEvent")]
    internal class EventPatch
    {
        public static void Postfix(ExitGames.Client.Photon.EventData photonEvent)
        {
            foreach (var eventCallback in BehaviourHandler.TryGetBehaviours<EventBehaviour>())
                eventCallback?.OnEvent(photonEvent.Code, photonEvent.Sender, photonEvent.CustomData);
        }
    }
}