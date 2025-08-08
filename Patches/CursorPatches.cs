using HarmonyLib;
using UnityEngine;

namespace PeakCheat.Patches
{
    public class CursorSettings
    {
        private static bool _forceShow = false;
        public static void ShowCursor(bool show)
        {
            _forceShow = show;
            Cursor.visible = show;
            Cursor.lockState = show? CursorLockMode.None: CursorLockMode.Locked;
        }
        [HarmonyPatch(typeof(CursorHandler), "Update")]
        class CursorPatch
        {
            private static bool Prefix() => !_forceShow;
        }
    }
}
