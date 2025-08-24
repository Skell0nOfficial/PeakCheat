using PeakCheat.Cheats.Fun;
using PeakCheat.Cheats.Gameplay;
using PeakCheat.Cheats.Miscellaneous;
using PeakCheat.Cheats.Movement;
using PeakCheat.Cheats.Visuals;
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
        private static int _currentCategory = 0;
        public static string CurrentCategory
        {
            get
            {
                if (_currentCategory >= _cheats.Count) _currentCategory = 0;

                return _cheats.ToArray()[_currentCategory].Key;
            }
        }
        private static readonly Dictionary<string, Type[]> _orderList = new Dictionary<string, Type[]>()
        {
            ["Abusive"] = new Type[] { },
            ["Fun"] = new[]
            {
                typeof(BingBomb),
                typeof(ExplosivePoints)
            },
            ["Gameplay"] = new[]
            {
                typeof(Immortality),
                typeof(ReviveAll)
            },
            ["Miscellaneous"] = new[]
            {
                typeof(Anticrash),
                typeof(DebugScreen)
            },
            ["Movement"] = new[]
            {
                typeof(AirJump),
                typeof(Fly),
                typeof(SpeedyClimb),
                typeof(StaminaHack),
                typeof(SuperSpeed)
            },
            ["Visuals"] = new[]
            {
                typeof(DeadEyes)
            }
        };
        private static readonly Dictionary<string, List<Cheat>> _cheats = new Dictionary<string, List<Cheat>>();
        public static Cheat[] Cheats => _cheats.Values.SelectMany(X => X).Where(C => !C.Hide()).ToArray();
        static string GetCategory(Type cheatType) => (cheatType.Namespace?? "Unknown").Replace("PeakCheat.Cheats", "").Replace('.', ' ').Trim();
        bool CheatBehaviour.DelayStart() => true;
        void CheatBehaviour.Start()
        {
            var categories = "PeakCheat.Cheats";
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(T => T.Namespace.StartsWith(categories) && T.IsSubclassOf(typeof(Cheat))).ToArray();
            var dict = new Dictionary<string, List<Type>>();

            LogUtil.Log($"Found [{types.Length}] Cheat Types");

            foreach (var type in types)
            {
                var space = GetCategory(type);
                LogUtil.Log($"{type.Name}: {space}");
                if (string.IsNullOrEmpty(space)) continue;

                if (!dict.TryGetValue(space, out var list))
                {
                    list = type.SingleList();
                    dict[space] = list;
                }

                list.AddIfNew(type);
            }

            LogUtil.Log($"\n\n# Variable Data #\nCategories: {dict.Count}\nCheats: {dict.Values.SelectMany(X => X).ToArray().Length}\n\n".ToUpper());

            foreach (var pair in dict)
            {
                var category = pair.Key;
                var cheatList = pair.Value.OrderBy(T => GetOrder(category, T, out int i)? i: -1);

                if (!_cheats.TryGetValue(category, out var cheats))
                {
                    cheats = new List<Cheat>();
                    _cheats[category] = cheats;
                }

                foreach (var type in cheatList.DeleteDuplicates())
                {
                    var cheat = (Cheat)Activator.CreateInstance(type);
                    if (cheat.DefaultEnabled) Toggle(cheat);
                    Debug.Log($"Added Cheat {cheat.Name} ({category})");
                    cheats.Add(cheat);
                }
            }
        }
        void CheatBehaviour.Update()
        {
            foreach (var cheat in Cheats)
                if (cheat.Enabled && IsAllowed(cheat))
                    cheat.Method();
        }
        public static bool GetOrder(string category, Type cheat, out int index)
        {
            if (_orderList.TryGetValue(category, out var list))
            {
                index = Array.IndexOf(list, cheat);
                return true;
            }
            index = -1;
            return false;
        }
        public static bool IsEnabled<T>() where T : Cheat => TryGetCheat<T>(out var c) && c != null && c.Enabled;
        public static bool TryGetCheat<T>(out T? cheat)where T : Cheat
        {
            if (TryGetCheat(typeof(T), out var c) && c != null)
            {
                cheat = (T)c;
                return true;
            }
            cheat = null;
            return false;
        }
        public static bool TryGetCheat(Type type, out Cheat? cheat) => Cheats.ToDictionary(C => C.GetType()).TryGetValue(type, out cheat);
        public override void Render()
        {
            var categorySize = new Vector2(Data.Width * .95f, Data.Height / 8f);
            var categoryRect = new Rect(Vector2.zero, categorySize);

            if (GUI.Button(categoryRect, CurrentCategory.Bold(64), UnityUtil.CreateStyle(GUI.skin.button, Color.clear))) _currentCategory++;

            var spacing = .025f;
            var cheats = _cheats[CurrentCategory];
            var maxWidth = Data.Width * .95f;
            var size = new Vector2(maxWidth / 3f, 50f);
            var positions = UnityUtil.GenerateLinePositions(cheats.Count, spacing, Data.Width, size);
            
            int num = 0;
            foreach (var pos in positions)
            {
                var cheat = cheats[num++];
                var color = cheat.Enabled?
                    (Color.white * .025f).WithAlpha(1f):
                    (Color.white * .017f).WithAlpha(.85f);
                var style = UnityUtil.CreateStyle(GUI.skin.button, color);
                var rect = Rect.zero;

                rect.position = (Vector2.up * categorySize.y) + pos + (Vector2.right * (size.y * spacing));
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