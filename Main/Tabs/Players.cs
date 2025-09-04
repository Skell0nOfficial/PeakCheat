using PeakCheat.Types;
using PeakCheat.Utilities;
using Photon.Pun;
using System;
using System.Linq;
using UnityEngine;

namespace PeakCheat.Main.Tabs
{
    internal class Players : UITab, CheatBehaviour
    {
        public override string Name => "Players";
        public override int Order => 2;
        private enum PlayerType
        {
            None,
            All,
            Host,
            Others,
            Except,
            Certain,
            Myself
        }
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

            bool RenderButton(string text, float height = 18f) => RenderButtonContent(new GUIContent(text));
            bool RenderButtonContent(GUIContent data, float height = 18f) => GUILayout.Button(data, GUI.skin.button.CreateStyle((Color.white * .01f).WithAlpha(.8f)), GUILayout.Width(width), GUILayout.Height(Data.Height / height));


            GUILayout.BeginVertical();
            scroller = GUILayout.BeginScrollView(scroller, GUI.skin.scrollView.CreateStyle(Color.clear), GUILayout.Width(Data.Width * .95f), GUILayout.Height(Data.Size.y * .8f));
            text = GUILayout.TextField(text, GUILayout.Width(width), GUILayout.Height(Data.Height / 17f));
            GUILayout.Label("123", GUILayout.Width(width), GUILayout.Height(Data.Height / 20f));

            var tex = text.ToLower().Trim();
            var type = PlayerType.None;
            object? arguments = null;

            void RunScript(Action<CheatPlayer> action)
            {
                void print(object message) => LogUtil.Log(true, $"[Evalulator] {message}");
                switch (type)
                {
                    case PlayerType.All: PlayerUtil.AllPlayers().Execute(action); break;
                    case PlayerType.Others: PlayerUtil.OtherPlayers().Execute(action); break;
                    case PlayerType.Host: action(PhotonNetwork.MasterClient); break;
                    case PlayerType.Myself: action(CheatPlayer.LocalPlayer); break;

                    case PlayerType.Except:
                    case PlayerType.Certain:
                        if (arguments is CheatPlayer[] Players)
                            foreach (var player in PlayerUtil.AllPlayers())
                            {
                                if (player == null) continue;
                                if ((type == PlayerType.Certain && !Players.Contains(player)) || (type == PlayerType.Except && Players.Contains(player))) continue;

                            }
                        break;

                    default: print($"Type [{type}] is unsupported"); break;
                }
            }

            if (type == PlayerType.None)
            {
                Error("Evalulation", "Unable to decrypt message arguments");

                GUILayout.EndVertical();
                GUILayout.EndScrollView();
                return;
            }

            foreach (var pair in PlayerScript.GetMethods())
            {
                if (RenderButton(pair.Key))
                {
                    AudioUtil.Click();
                    RunScript(player => PlayerScript.ExecuteScript(player, pair));
                }
            }
            foreach (var item in ItemList.OrderBy(I => I.GetName()))
            {
                var name = item.UIData.itemName;
                var objName = item.name.Replace("(Clone)", "");

                bool Button(string type) => RenderButtonContent(new GUIContent($"{type} {name}", item.UIData.icon), 14);

                if (Button("Give")) RunScript(C => C.GiveItem(objName));
                if (Button("Drop")) RunScript(C => PhotonNetwork.InstantiateItem(objName, C.Center + (C.BodyTransform?.forward * .4f ?? Vector3.forward), Quaternion.identity));

                string action = item.UIData.mainInteractPrompt.Trim().ToLower();
                if ((action == "eat" || action == "drink") && Button("Feed")) RunScript(C => C.FeedItem(objName));
                if ((action == "use" || action == "commune") && Button("Use")) RunScript(C => C.FeedItem(objName));
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
    }
}