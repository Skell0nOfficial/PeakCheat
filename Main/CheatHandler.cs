using PeakCheat.Cheats.Fun;
using PeakCheat.Cheats.Gameplay;
using PeakCheat.Cheats.Miscellaneous;
using PeakCheat.Cheats.Movement;
using PeakCheat.Cheats.Visuals;
using PeakCheat.Types;
using PeakCheat.Utilities;
using Photon.Pun;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zorro.UI.Modal;

namespace PeakCheat.Main
{
    public class CheatHandler : UITab, CheatBehaviour
    {
        public override string Name => "Cheats";
        public override int Order => 1;
        private const string SaveKey = "CheatHandler::SavingCheats";
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
        private static Vector2 _scroller = Vector2.zero;
        public static Cheat[] Cheats => _cheats.Values.SelectMany(X => X).Where(C => !C.Hide()).ToArray();
        static string GetCategory(Type cheatType) => (cheatType.Namespace?? "Unknown").Replace("PeakCheat.Cheats", "").Replace('.', ' ').Trim();
        public static void SaveCheats() => Cheats.Where(C => C.Enabled).Select(C => C.GetID()).ToList().Save(SaveKey);
        bool CheatBehaviour.DelayStart() => true;
        void CheatBehaviour.Start()
        {
            _cheats.Clear();

            var categories = "PeakCheat.Cheats";
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(T => T != null && !string.IsNullOrEmpty(T.Namespace) && T.Namespace.StartsWith(categories) && T.IsSubclassOf(typeof(Cheat))).ToArray();
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

            bool gotSave = SaveUtil.Get(SaveKey, out List<int> savedKeys);
            foreach (var pair in dict)
            {
                var category = pair.Key;
                var cheatList = pair.Value.OrderBy(T => GetOrder(category, T, out int i)? i: -1);

                if (!_cheats.TryGetValue(category, out var cheats))
                {
                    cheats = new List<Cheat>();
                    _cheats[category] = cheats;
                }

                foreach (var type in cheatList.DeleteDuplicates(C => $"{C.Namespace}.{C.Name}"))
                {
                    var cheat = (Cheat)Activator.CreateInstance(type);
                    if (cheat.DefaultEnabled && !gotSave) Toggle(cheat);
                    Debug.Log($"Added Cheat {cheat.Name} ({category})");
                    cheats.Add(cheat);
                }
            }

            if (gotSave)
                foreach (var cheat in Cheats)
                    if (savedKeys.Contains(cheat.GetID()) && !cheat.Enabled)
                        Toggle(cheat);
        }
        void CheatBehaviour.Update()
        {
            if (TimeUtil.CheckTime(3f) && PhotonNetwork.InRoom)
            {
                var service = GameHandler.GetService<SteamLobbyHandler>();
                if (service.InSteamLobby()) service.LeaveLobby();
            }
            foreach (var cheat in Cheats)
                if (cheat.Enabled && SceneAllowed(cheat))
                    try { cheat.Method(); } catch {}
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
            var scrollStyle = GUIStyle.none;
            _scroller = GUILayout.BeginScrollView(_scroller, false, true, scrollStyle, scrollStyle);
            GUILayout.BeginVertical();

            foreach (var pair in _cheats)
            {
                GUILayout.Label(pair.Key.Bold(20));

                foreach (var cheat in pair.Value)
                {
                    if (cheat.Hide()) continue;
                    void SetMessage(string msg, bool success)
                    {
                        cheat.SetData(223, Time.time);
                        cheat.SetData(224, msg);
                        cheat.SetData(225, success);
                    }

                    var enabled = cheat.Enabled;
                    var style = GUI.skin.button.CreateStyle((Color.white * (enabled ? .025f : .017f)).WithAlpha(enabled ? 1f : .85f));
                    var content = GUIContent.Temp(cheat.Name.Bold());

                    if (cheat.GetData<bool>(225, out var b) && b is bool successMessage &&
                        cheat.GetData<string>(224, out var str) && str is string message &&
                        cheat.GetData<float>(223, out var time) && time is float messageTime &&
                        Time.time - messageTime <= (.05f * message.Length))
                        content = GUIContent.Temp($"<color=#{(successMessage? "2e9415" : "8c1f0e")}>{message}</color>".Bold());

                    var rect = GUILayoutUtility.GetRect(content, style, new GUILayoutOption[]
                    {
                        GUILayout.Width(Data.Width * .9f),
                        GUILayout.Height(Data.Height / 13f)
                    });

                    if (rect.Contains(Event.current.mousePosition))
                    {
                        if (Input.GetMouseButton(1)) content = GUIContent.Temp($"Description: {cheat.Description}".Bold());
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (!SceneAllowed(cheat))
                            {
                                SetMessage($"You cant use this module in scene {CheatUtil.CurrentScene}!", false);
                                return;
                            }

                            bool worked = Toggle(cheat, out var m);
                            if (m is string && !string.IsNullOrEmpty(m)) SetMessage(m, worked);
                        }
                    }

                    GUI.Button(rect, content, style);
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
        public static bool Toggle(Cheat cheat) => Toggle(cheat, out _);
        public static bool Toggle(Cheat cheat, out string? message)
        {
            if (!SceneAllowed(cheat))
            {
                message = $"{cheat.Name} is not allowed";
                return false;
            }
            if (!TimeUtil.CheckTime($"Toggle:{cheat.Name}", .1f))
            {
                message = null;
                return false;
            }

            message = null;
            cheat.Enabled = !cheat.Enabled;
            if (cheat.Enabled)
            {
                cheat.Enable();
                return true;
            }

            cheat.Disable();
            return true;
        }
        public static bool SceneAllowed(Cheat cheat) => (int)cheat.RequiredScene <= (int)CheatUtil.CurrentScene;
    }
}