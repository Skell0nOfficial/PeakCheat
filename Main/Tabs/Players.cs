using PeakCheat.Types;
using PeakCheat.Utilities;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PeakCheat.Main.Tabs
{
    internal class Players: UITab, CheatBehaviour
    {
        private static string text = "???";
        public override string Name => "Players";
        public override void Render()
        {
            void RenderText(string title, string header, string description)
            {
                var rect = Data.GetRect;
                Vector2 GetRect(float y) => Vector2.up * y;
                var titleRect = new Rect(GetRect(0f), rect.size);
                var headerRect = new Rect(GetRect(60f), rect.size);
                var textRect = new Rect(GetRect(100f), rect.size);

                GUI.Label(titleRect, title.Bold(50));
                GUI.Label(headerRect, header.Size(20));
                GUI.Label(textRect, description);
            }

            if (CheatUtil.CurrentScene < SceneType.Airport)
            {
                RenderText("Cant Render", "Scene Exception", $"You must be in the airport or a Level, Current Scene: {CheatUtil.CurrentScene}");
                return;
            }

            if (!PhotonNetwork.InRoom)
            {
                RenderText("Loading..", "Waiting for photon to establish a connection", $@"Information:
Current Ping: {PhotonNetwork.GetPing()}
Server IP: {PhotonNetwork.ServerAddress}
Proxy Server IP: {PhotonNetwork.NetworkingClient.ProxyServerAddress}
");
                return;
            }

            var p = Character.observedCharacter;
            var type = typeof(PlayerUtil);
            var width = Data.Width * .96f;

            if (!MainCameraMovement.IsSpectating)
            {
                text = GUILayout.TextField(text, GUILayout.Width(width), GUILayout.Height(Data.Height / 13f));
                if (PlayerUtil.AllPlayers().Any(P => P.Name.Contains(text), out var player)) p = player;
            }

            var methods = new List<MethodInfo>();

            foreach (var method in type.GetMethods())
            {
                if (method == null) continue;
                if (method.ReturnType != typeof(void) && method.ReturnType != typeof(bool)) continue;
                if (method.GetParameters().Length != 1) continue;
                var parameters = method.GetParameters();
                if (parameters.All(P => P.ParameterType == typeof(CheatPlayer))) methods.Add(method);
            }

            foreach (var method in methods)
                if (GUILayout.Button($"{method.Name}: {(p.IsLocal ? "Self" : p.characterName)}", UnityUtil.CreateStyle(GUI.skin.button, (Color.white * .008f).WithAlpha(.8f)), GUILayout.Width(width), GUILayout.Height(25f)))
                    method.Invoke(null, ((object)p.ToCheatPlayer()).SingleArray());
        }
    }
}