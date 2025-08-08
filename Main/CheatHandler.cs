using PeakCheat.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PeakCheat.Main
{
    public class CheatHandler: UITab, CheatBehaviour
    {
        public override string Name => "Cheats";
        private static Dictionary<string, List<Cheat>> _cheats = new Dictionary<string, List<Cheat>>();
        public static Cheat[] Cheats => _cheats.Values.SelectMany(X => X).Where(C => !C.Hide()).ToArray();
        private static Rect _rect = Rect.zero;
        private static bool _show = false;
        void CheatBehaviour.Start()
        {
            string cheatNames = "PeakCheat.Cheats";
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                if (type.IsSubclassOf(typeof(Cheat)) && !type.IsAbstract && type.Namespace.StartsWith(cheatNames))
                {
                    string category = type.Namespace.Replace(cheatNames + '.', "").Replace('.', ' ');
                    var cheat = (Cheat)Activator.CreateInstance(type);
                    Debug.Log($"[{category}] Added Cheat: {cheat.Name}");
                    if (_cheats.TryGetValue(category, out var cheats))
                    {
                        cheats.Add(cheat);
                        continue;
                    }
                    _cheats.Add(category, new Cheat[] { cheat }.ToList());
                }
        }
        void CheatBehaviour.Update()
        {
            if (BepInEx.UnityInput.Current.GetKeyDown(KeyCode.F2))
                _show = !_show;
            foreach (var cheat in Cheats)
                if (cheat.Enabled)
                    cheat.Method();
        }
        public void Toggle(Cheat cheat)
        {
            cheat.Enabled = !cheat.Enabled;
            if (cheat.Enabled) cheat.Enable();
            else cheat.Disable();
        }
    }
}