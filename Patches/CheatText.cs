using HarmonyLib;
using PeakCheat.Main;
using PeakCheat.Utilities;
using TMPro;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(VersionString), "Update")]
    internal class CheatText
    {
        static void Postfix(VersionString __instance)
        {
            if (__instance.TryGetComponent<TextMeshProUGUI>(out var text))
            {
                if (UIHandler.Open)
                {
                    text.text = "";
                    return;
                }
                
                text.text = $"PeakCheat V{Plugin.Instance?.Info.Metadata?.Version?.ToString() ?? "0.0"}";
                text.richText = true;
                text.text = CheatUtil.CurrentScene >= Types.SceneType.Airport? text.text.Bold(27): "";
            }
        }
    }
}