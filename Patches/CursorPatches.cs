using HarmonyLib;
using PeakCheat.Main;
using UnityEngine;

namespace PeakCheat.Patches
{
    public class CursorSettings
    {
        private static bool _show = false;
        public static bool ForceShow
        {
            get => UIHandler.Open || _show;
            set => _show = value;
        }
        public static void ShowCursor(bool show)
        {
            ForceShow = show;
            Cursor.visible = show;
            Cursor.lockState = show? CursorLockMode.None: CursorLockMode.Locked;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        [HarmonyPatch(typeof(CursorHandler), "Update")]
        class CursorPatch
        {
            private static bool Prefix() => !ForceShow;
        }
        [HarmonyPatch(typeof(CursorAnimation), "Update")]
        class CursorPatch2
        {
            private static bool Prefix() => !ForceShow;
        }
    }
}
