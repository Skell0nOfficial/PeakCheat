using HarmonyLib;
using PeakCheat.Main;
using UnityEngine;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(MainCameraMovement), "SwapSpecPlayer")]
    internal class SpectatePatch
    {
        static bool Prefix() => !UIHandler.Open && !Cursor.visible;
    }
}