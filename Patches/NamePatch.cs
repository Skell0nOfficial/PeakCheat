using HarmonyLib;
using Photon.Pun;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(PhotonNetwork), "NickName", MethodType.Setter)]
    internal class NamePatch
    {
        private static string? _name = null;
        private static void Prefix(ref string value)
        {
            if (!string.IsNullOrEmpty(_name))
                value = _name;
        }
    }
}
