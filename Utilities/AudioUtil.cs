using HarmonyLib;
using Photon.Voice.Unity;

namespace PeakCheat.Utilities
{
    internal class AudioUtil
    {
        private static Recorder _microphone = null;
        [HarmonyPatch(typeof(VoiceClientHandler), nameof(VoiceClientHandler.LocalPlayerAssigned))]
        class MicPatch
        {
            private static void Postfix(Recorder r) => SetRecorder(r);
        }
        public static void SetRecorder(Recorder recorder)
        {

        }
    }
}