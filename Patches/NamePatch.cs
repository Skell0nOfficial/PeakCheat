using HarmonyLib;
using PeakCheat.Types;
using PeakCheat.Utilities;
using System.Linq;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(Character), "characterName", MethodType.Getter)]
    public class NamePatch
    {
        static void Postfix(Character __instance, ref string __result)
        {
            if (__instance.ToCheatPlayer() is CheatPlayer player)
                if (player.Flags.Length > 0)
                    __result += $" ({string.Join(", ", player.Flags.Select(F => F.ToString()).ToArray())})";
        }
    }
}