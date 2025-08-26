using PeakCheat.Types;
using PeakCheat.Utilities;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PeakCheat.Main.Tabs
{
    internal class Players: UITab, CheatBehaviour
    {
        public override string Name => "Players";
        private static int _currentPlayer = -1;
        private static string text = "???";
        private static Vector2 scroller = Vector2.zero;
        public override void Render()
        {
            void RenderText(string title, string header, string description)
            {
                var rect = Data.GetRect;

                Vector2 GetRect(float y) => Vector2.up * y;
                var titleRect = new Rect(GetRect(0f), rect.size);
                var headerRect = new Rect(GetRect(60f), rect.size);
                var textRect = new Rect(GetRect(100f), rect.size);

                GUI.skin.label.richText = true;
                GUI.Label(titleRect, title.Bold(50));
                GUI.Label(headerRect, header.Size(20));
                GUI.Label(textRect, description);
            }
            void Error(string type, string text) => RenderText("Cant Render", $"{type} Exception", text);

            if (CheatUtil.CurrentScene < SceneType.Airport)
            {
                Error("Scene", $"Cant Execute in scene: {CheatUtil.CurrentScene}");
                return;
            }
            if (!PhotonNetwork.InRoom)
            {
                RenderText("Loading..", "Waiting for photon to establish a connection", $@"Information:
Current Ping: {PhotonNetwork.GetPing()}
Server IP: {PhotonNetwork.ServerAddress}
Server Type: {PhotonNetwork.Server}
");
                return;
            }

            void PlayerError(string error)
            {
                Error("Player", error);
                _currentPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
            }

            var width = Data.Width * .9f;
            text = GUILayout.TextField(text, GUILayout.Width(width), GUILayout.Height(Data.Height / 17f));

            var methods = new List<MethodInfo>();
            var player = Character.observedCharacter;

            if (PlayerUtil.AllPlayers().Any(C => C.Name.ToLower().Trim().Contains(text.ToLower().Trim()), out var c)) player = c;
            if (player == null)
            {
                PlayerError("Cant find a player!?");
                return;
            }

            foreach (var type in new Type[] { typeof(PlayerUtil), typeof(ForceFeedUtil) })
                foreach (var method in type.GetMethods())
                {
                    if (method == null) continue;
                    if (method.DeclaringType != type) continue;
                    if (method.Name.Contains("get_")) continue;
                    if (!method.IsStatic) continue;
                    if (method.GetParameters().Length != 1) continue;

                    if (method.GetParameters()[0].ParameterType == typeof(CheatPlayer))
                    {
                        methods.Add(method);
                        continue;
                    }

                    if (method.GetParameters()[0].ParameterType == typeof(Vector3)) methods.Add(method);
                }

            scroller = GUILayout.BeginScrollView(scroller, UnityUtil.CreateStyle(GUI.skin.scrollView, Color.clear), GUILayout.Width(Data.Width * .95f), GUILayout.Height(Data.Size.y * .8f));
            GUILayout.BeginVertical();

            foreach (var method in methods)
                if (GUILayout.Button($"{method.Name}({$"{nameof(CheatPlayer)} {player.characterName.ToLower()}"});", UnityUtil.CreateStyle(GUI.skin.button, Color.black), GUILayout.Width(width), GUILayout.Height(Data.Height / 18f)))
                {
                    object[] objects = new object[0];
                    if (method.GetParameters().Length == 1)
                    {
                        var param = method.GetParameters()[0];
                        if (param == null) continue;
                        if (param.ParameterType == typeof(Vector3)) objects = new object[] { player.Center };
                        else if (param.ParameterType == typeof(CheatPlayer)) objects = ((object)player.ToCheatPlayer()).SingleArray();
                    }

                    var result = method.Invoke(null, objects);
                    if (method.ReturnType == typeof(void))
                    {
                        LogUtil.Log($"Invoked {method.Name}(), no return value");
                        continue;
                    }

                    if (result is bool value)
                    {
                        LogUtil.Log($"Invoked {method.Name}(), result: {value.ToString().ToLower()}");
                        continue;
                    }

                    LogUtil.Log($"Invoked {method.Name}(), result: {result}");
                }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

           /*if (_currentPlayer <= 0) _currentPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
            if (!PhotonNetwork.TryGetPlayer(_currentPlayer, out var player))
            {
                PlayerError("TryGetPlayer is false");
                return;
            }

            var current = player.ToCheatPlayer();
            if (current == null)
            {
                PlayerError("current is null");
                return;
            }
            
            GUILayout.BeginVertical();
            GUILayout.Space(10f);
            
            GUILayout.EndVertical();*/
        }
    }
}