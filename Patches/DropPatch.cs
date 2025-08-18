using HarmonyLib;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(CharacterItems), "Equip")]
    public class DropPatch
    {
        public static void Postfix(CharacterItems __instance, Item item)
        {
            if (__instance.TryGetComponent<Character>(out var c) && c.IsLocal && item.TryGetComponent<Action_Passport>(out _))
            {
                item.UIData.canDrop = true;
                item.UIData.canThrow = true;
            }
        }
    }
}