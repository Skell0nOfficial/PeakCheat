using PeakCheat.Classes;
using PeakCheat.Patches;
using PeakCheat.Utilities;
using UnityEngine;

namespace PeakCheat.Main
{
    public class UIHandler: MonoBehaviour, CheatBehaviour
    {
        private static bool _opened = false;
        public static bool GUIActive() => _opened;
        void CheatBehaviour.Start()
        {
            foreach (var color in UnityUtil.EveryColor()) UnityUtil.FromColor(color, false);

            LogUtil.Log("Generated color textures");
        }
        void CheatBehaviour.Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _opened = !_opened;
                CursorSettings.ShowCursor(_opened);
                LogUtil.Log(_opened? "GUI Initialized": "Closed GUI");
            }
        }
        private void OnGUI()
        {
            if (!_opened) return;

            // adding the actual UI tmrw (next commit)
        }
    }
}