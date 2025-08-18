using PeakCheat.Types;
using PeakCheat.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PeakCheat.Main
{
    public class CheatHandler : UITab, CheatBehaviour
    {
        public override string Name => "Cheats";
        private static string? _currentCategory = null;
        private static readonly Dictionary<string, List<Cheat>> _cheats = new Dictionary<string, List<Cheat>>();
        public static Cheat[] Cheats => _cheats.Values.SelectMany(X => X).Where(C => !C.Hide()).ToArray();
        void CheatBehaviour.Start()
        {
            string cheatNames = "PeakCheat.Cheats";
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                if (type.IsSubclassOf(typeof(Cheat)) && !type.IsAbstract && type.Namespace.StartsWith(cheatNames))
                {
                    string category = type.Namespace.Replace(cheatNames + '.', "").Replace('.', ' ');
                    var cheat = (Cheat)Activator.CreateInstance(type);
                    Debug.Log($"Added Cheat {cheat.Name} ({category})");
                    if (_cheats.TryGetValue(category, out var cheats))
                    {
                        cheats.Add(cheat);
                        continue;
                    }
                    _cheats.Add(category, cheat.SingleList());
                }
        }
        void CheatBehaviour.Update()
        {
            foreach (var cheat in Cheats)
                if (cheat.Enabled && IsAllowed(cheat))
                    cheat.Method();
        }
        public override void Render()
        {
            if (string.IsNullOrEmpty(_currentCategory)) _currentCategory = _cheats.First().Key;

            var spacing = .1f;
            var cheats = Cheats.ToList();
            var size = new Vector2(90f, 50f);
            var positions = UnityUtil.GenerateLinePositions(cheats.Count, spacing, Data.Width, size);
            
            int num = 0;
            foreach (var pos in positions)
            {
                var cheat = cheats[num++];
                var color = cheat.Enabled ?
                    (Color.white * .025f).WithAlpha(1f) :
                    (Color.white * .017f).WithAlpha(.85f);
                var style = UnityUtil.CreateStyle(GUI.skin.button, color);
                var rect = Rect.zero;

                rect.position = pos + (Vector2.right * (size.y * spacing));
                rect.size = size;

                if (GUI.Button(rect, cheat.Name, style)) Toggle(cheat);
            }
        }
        public static void Toggle(Cheat cheat)
        {
            cheat.Enabled = !cheat.Enabled;
            if (cheat.Enabled)
            {
                cheat.Enable();
                return;
            }
            cheat.Disable();
        }
        public static bool IsAllowed(Cheat cheat) => (int)cheat.RequiredScene <= (int)CheatUtil.CurrentScene;
    }
}