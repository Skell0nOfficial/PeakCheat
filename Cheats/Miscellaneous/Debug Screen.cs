using PeakCheat.Classes;
using PeakCheat.Utilities;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

namespace PeakCheat.Cheats.Miscellaneous
{
    internal class DebugScreen: Cheat
    {
        public override string Name => "Debug Screen";
        public override string Description => "Toggles the DebugUI used by LandCrab with [F3]";
        private static DebugUIHandler? _handler;
        public override void Enable()
        {
            if (!GetUI(out var ui))
            {
                LogUtil.Log(false, "Could not fetch DebugUI");
                Enabled = false;
                return;
            }
            LogUtil.Log($"Got DebugUI: {ui.name}");
        }
        public override void Method()
        {
            if (Input.GetKeyDown(KeyCode.F3) && GetUI(out var ui))
                if (ui.IsOpened)
                    ui.Hide();
                else
                    ui.Show();
        }
        private static bool GetUI(out DebugUIHandler handler)
        {
            _handler = Singleton<DebugUIHandler>.Instance;
            if (_handler != null && _handler is DebugUIHandler debugUIHandler)
            {
                handler = debugUIHandler;
                return true;
            }
            handler = null;
            return false;
        }
    }
}