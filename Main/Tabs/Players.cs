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
    internal class Players : UITab, CheatBehaviour
    {
        public override string Name => "Players";
        public override int Order => 2;
        private static string text = UnityEngine.Random.Range(10000, 99999).ToString();
        private static Vector2 scroller = Vector2.zero;
        private static Item[] ItemList = Array.Empty<Item>();
        public override void Toggle(bool toggled) => ItemList = Resources.FindObjectsOfTypeAll<Item>().Where(C => !C.name.Contains("(Clone)")).DeleteDuplicates(I => I.itemID).ToArray();
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
            void PlayerError(string error)
            {
                Error("Player", error);
            }
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

            var width = Data.Width * .9f;
            var player = Character.observedCharacter;

            if (PlayerUtil.AllPlayers().Any(C => C.Name.ToLower().Trim().Contains(text.ToLower().Trim()), out var c)) player = c;
            if (player == null)
            {
                PlayerError("Cant find a player!?");
                return;
            }

            GUILayout.BeginVertical();
            scroller = GUILayout.BeginScrollView(scroller, UnityUtil.CreateStyle(GUI.skin.scrollView, Color.clear), GUILayout.Width(Data.Width * .95f), GUILayout.Height(Data.Size.y * .8f));
            text = GUILayout.TextField(text, GUILayout.Width(width), GUILayout.Height(Data.Height / 17f));
            GUILayout.Label($"Current Player: {player.characterName}", GUILayout.Width(width), GUILayout.Height(Data.Height / 20f));
            bool RenderButton(string text) => RenderButtonContent(new GUIContent(text));
            bool RenderButtonContent(GUIContent data) => GUILayout.Button(data, UnityUtil.CreateStyle(GUI.skin.button, Color.black), GUILayout.Width(width), GUILayout.Height(Data.Height / 14f));

            foreach (var pair in PlayerScript.GetMethods())
            {
                var name = pair.Key;
                var method = pair.Value;

                if (RenderButton(name))
                {
                    object[] objects = new object[0];
                    if (method.GetParameters().Length == 1)
                    {
                        var param = method.GetParameters()[0];
                        if (param == null) continue;
                        if (param.ParameterType == typeof(Vector3)) objects = new object[] { player.Center };
                        else if (param.ParameterType == typeof(CheatPlayer)) objects = ((object)player.ToCheatPlayer()).SingleArray();
                    }

                    try
                    {
                        var result = method.Invoke(null, objects);

                        if (method.ReturnType == typeof(void))
                        {
                            LogUtil.Log($"Invoked {name}, no return value");
                            continue;
                        }

                        if (result is bool value)
                        {
                            LogUtil.Log($"Invoked {name}, result: {value.ToString().ToLower()}");
                            continue;
                        }

                        LogUtil.Log($"Invoked {name}, result: {result}");
                    }
                    catch (Exception error)
                    {
                        LogUtil.Log(true, $@"
CANT INVOKE {name.ToUpper()}


Message: {error.Message}
Source: {error.Source}");
                    }
                }
            }
            foreach (var item in ItemList)
            {
                var name = item.UIData.itemName;
                var objName = item.name.Replace("(Clone)", "");
                var C = player.ToCheatPlayer();
                bool Button(string type) => RenderButtonContent(new GUIContent($"{type} {name}", item.UIData.icon));

                if (Button("Give")) C.GiveItem(objName);
                if (Button("Drop")) PhotonNetwork.InstantiateItem(objName, C.Center + (C.BodyTransform?.forward * 1.5f ?? Vector3.forward), Quaternion.identity);
                
                string action = item.UIData.mainInteractPrompt.Trim().ToLower();
                if ((action == "eat" || action == "drink") && Button("Feed")) C.FeedItem(objName);
                if ((action == "use" || action == "commune") && Button("Use")) C.FeedItem(objName);
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
    }
}