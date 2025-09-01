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
        private enum Player
        {
            All,
            Others,
            Host
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
            var specific = true;
            var others = false;
            var tex = text.ToLower().Trim();

            bool RenderButton(string text, float height = 18f) => RenderButtonContent(new GUIContent(text));
            bool RenderButtonContent(GUIContent data, float height = 18f) => GUILayout.Button(data, GUI.skin.button.CreateStyle((Color.white * .01f).WithAlpha(.8f)), GUILayout.Width(width), GUILayout.Height(Data.Height / height));

            if (tex.Contains("array:"))
            {
                specific = false;
                others = !text.Split(':')[1].StartsWith("a");
            }
            
            if (PlayerUtil.AllPlayers().Any(C => C.Name.ToLower().Trim().Contains(tex), out var c)) player = c;
            if (player == null)
            {
                PlayerError("Cant find a player!?");
                return;
            }

            GUILayout.BeginVertical();
            scroller = GUILayout.BeginScrollView(scroller, GUI.skin.scrollView.CreateStyle(Color.clear), GUILayout.Width(Data.Width * .95f), GUILayout.Height(Data.Size.y * .8f));
            text = GUILayout.TextField(text, GUILayout.Width(width), GUILayout.Height(Data.Height / 17f));
            GUILayout.Label(specific? $"Current Player: {player.characterName}": others? "Others": "All", GUILayout.Width(width), GUILayout.Height(Data.Height / 20f));

            foreach (var pair in PlayerScript.GetMethods())
            {
                if (RenderButton(pair.Key))
                {
                    AudioUtil.Click();

                    if (specific)
                    {
                        PlayerScript.ExecuteScript(player, pair);
                        continue;
                    }

                    (others ? PlayerUtil.OtherPlayers() : PlayerUtil.AllPlayers()).Execute(C => PlayerScript.ExecuteScript(C, pair));
                }
            }
            foreach (var item in ItemList.OrderBy(I => I.GetName()))
            {
                var name = item.UIData.itemName;
                var objName = item.name.Replace("(Clone)", "");

                void Run(Action<CheatPlayer> action)
                {
                    if (specific)
                    {
                        action(player.ToCheatPlayer());
                        return;
                    }

                    (others ? PlayerUtil.OtherPlayers() : PlayerUtil.AllPlayers()).Execute(C => action(C));
                }

                bool Button(string type) => RenderButtonContent(new GUIContent($"{type} {name}", item.UIData.icon), 14);

                if (Button("Give")) Run(C => C.GiveItem(objName));
                if (Button("Drop")) Run(C => PhotonNetwork.InstantiateItem(objName, C.Center + (C.BodyTransform?.forward * .4f ?? Vector3.forward), Quaternion.identity));

                string action = item.UIData.mainInteractPrompt.Trim().ToLower();
                if ((action == "eat" || action == "drink") && Button("Feed")) Run(C => C.FeedItem(objName));
                if ((action == "use" || action == "commune") && Button("Use")) Run(C => C.FeedItem(objName));
            }
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
    }
}