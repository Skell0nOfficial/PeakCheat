using PeakCheat.Classes;
using PeakCheat.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PeakCheat.Main
{
    public class CheatHandler: CheatBehaviour
    {
        private static Dictionary<string, List<Cheat>> _cheats = new Dictionary<string, List<Cheat>>();
        public static Cheat[] Cheats => _cheats.Values.SelectMany(X => X).Where(C => !C.Hide()).ToArray();
        private static Rect _rect = Rect.zero;
        private static bool _show = false;
        public override void Start()
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
        public override void Update()
        {
            if (BepInEx.UnityInput.Current.GetKeyDown(KeyCode.F2))
                _show = !_show;
            foreach (var cheat in Cheats)
                if (cheat.Enabled)
                    cheat.Method();
        }
        public override void RenderUI()
        {
            if (!_show) return;
            var cheats = Cheats;
            int buttonsPerRow = 7;
            Vector2 buttonSize = _rect.size / buttonsPerRow;
            buttonSize.y *= .4f;
            var positions = UnityUtil.GenerateLinePositions(cheats.Length, buttonSize.x * buttonsPerRow, buttonSize);
            _rect.size = Vector2.one * 800f;
            _rect.center = new Vector2(Screen.width, Screen.height) / 2f;
            GUI.backgroundColor = Color.black;
            GUI.skin.label.richText = true;
            GUI.Window(409323, _rect, ID =>
            {
                foreach (var cheat in cheats)
                {
                    var pos = positions[Array.IndexOf(cheats, cheat)];
                    var buttonRect = new Rect(pos, buttonSize);
                    var buttonColor = cheat.Enabled? Color.white * .07f : Color.black;

                    if (GUI.Button(buttonRect, $"<b><size=13>{cheat.Name}</size></b>", UnityUtil.GetButton(buttonColor, buttonColor)))
                    {
                        cheat.Enabled = !cheat.Enabled;
                        if (cheat.Enabled) cheat.Enable();
                        else cheat.Disable();
                    }
                    var mousePos = Event.current.mousePosition;
                    if (buttonRect.Contains(mousePos))
                        GUI.Label(new Rect(mousePos - (Vector2.down * 7.4f), Vector2.one * 500f), $"<b><i><size=12>{cheat.Description}</size></i></b>");
                }
            }, "");
            GUI.DrawTexture(_rect, UnityUtil.FromColor(Color.black));
            GUI.Label(new Rect(_rect.position + (Vector2.down * 27f), _rect.size), "<b><size=23>PeakCheat</size></b>");
        }
    }
}