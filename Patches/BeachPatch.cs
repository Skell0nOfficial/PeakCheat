using HarmonyLib;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(Character), "StartPassedOutOnTheBeach")]
    internal class BeachPatch
    {
        static void Postfix()
        {
            if (Character.localCharacter?.data is CharacterData dat)
            {
                dat.passOutValue = 0f;
                dat.passedOut = false;
                dat.fullyPassedOut = false;
                dat.passedOutOnTheBeach = 0f;
                dat.lastPassedOut = float.MinValue;
                Character.localCharacter?.RPCA_UnFall();
            }
        }
    }
}