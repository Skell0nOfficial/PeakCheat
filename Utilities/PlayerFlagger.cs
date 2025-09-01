using ExitGames.Client.Photon;
using PeakCheat.Types;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public class PlayerFlagger: UITab, EventBehaviour, CheatBehaviour
    {
        public override string Name => "Flagging";
        private static readonly string[] _blacklistedProps = new string[]
        {
            "AtlUser",
            "AtlOwner",
            "CherryUser",
            "CherryOwner"
        };
        public const string ForcedPropertyKey = "PeakCheat::ForcedProperty";
        private const string SaveKey = "PlayerFlagger::Settings";
        private static Dictionary<Action, bool[]> _settings = new Dictionary<Action, bool[]>();
        private static Vector2 _scroller = Vector2.zero;
        private enum Action
        {
            Trap,
            Crash,
            Break_Game,
            Destroy_Player
        }
        public override void Render()
        {
            _scroller = GUILayout.BeginScrollView(_scroller, GUILayout.Width(Data.Width * .9f), GUILayout.Height(Data.Height * .9f));
            GUILayout.BeginVertical();
            if (CheatPlayer.Others.Any(C => C.Flags.Length > 0))
            {
                GUILayout.Label("Players".Bold(40));
                foreach (var player in CheatPlayer.Others)
                    if (player.Flags.Length > 0)
                        GUILayout.Label($"{player.Name} [{string.Join(", ", player.Flags.Select(F => F.ToString()).ToArray())}]".Bold());
            }

            GUILayout.Label("Settings".Bold(40));
            foreach (Action action in Enum.GetValues(typeof(Action)))
            {
                if (!_settings.TryGetValue(action, out var settings))
                {
                    _settings[action] = new bool[Enum.GetValues(typeof(CheatPlayer.PlayerFlag)).Length].Select(B => false).ToArray();
                    settings = _settings[action];
                }

                for (int i = 0; i < settings.Length; i++)
                {
                    var flag = (CheatPlayer.PlayerFlag)i;
                    var enabled = settings[i];
                    var name = action.ToString().Replace('_', ' ');

                    if (flag == CheatPlayer.PlayerFlag.Anticheat && action == Action.Destroy_Player) continue;
                    
                    bool clicked = GUILayout.Toggle(enabled, $"Automatically [{name}] for \"{flag}\" Users");
                    if (enabled != clicked)
                    {
                        if (clicked)
                            foreach (var player in CheatPlayer.Others)
                                if (player.HasFlag(flag))
                                    OnFlag(player, flag);

                        AudioUtil.Click();
                        LogUtil.Log("Saved flagging settings");
                    }
                    settings[i] = clicked;
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
        bool CheatBehaviour.DelayStart() => true;
        void CheatBehaviour.Start()
        {
            if (SaveUtil.Get(SaveKey, out Dictionary<Action, bool[]> dict))
            {
                LogUtil.Log("Got Flagging Settings");
                _settings = dict;
            }
            CheatPlayer.FlagCallback(OnFlag);
            PhotonCallbacks.PropertiesUpdate += CheatCheck;
        }
        void CheatBehaviour.Update()
        {
            if (TimeUtil.CheckTime(3f))
            {
                foreach (var player in PlayerUtil.AllPlayers())
                    if (player.PhotonPlayer is Photon.Realtime.Player photonPlayer)
                        CheatCheck(player, photonPlayer.CustomProperties);
                SaveUtil.Save(_settings, SaveKey);
            }
        }
        void EventBehaviour.OnEvent(byte code, int sender, object data)
        {
            if (!PhotonNetwork.InRoom) return;

            if (code == 69 && PhotonNetwork.TryGetPlayer(sender, out var p) && p.ToCheatPlayer() is CheatPlayer player && !player.HasFlag(CheatPlayer.PlayerFlag.Anticheat))
            {
                LogUtil.Log($"Found Anticheat User: {player.Name}");
                player.AddFlag(CheatPlayer.PlayerFlag.Anticheat);
            }
        }
        private async void OnFlag(CheatPlayer player, CheatPlayer.PlayerFlag type)
        {
            await Task.Delay(1000);
            foreach (var action in _settings.Keys.ToArray())
                if (_settings.TryGetValue(action, out var value))
                {
                    int index = (int)type;
                    if (value.Length <= index) continue;
                    if (value[index])
                        switch (action)
                        {
                            case Action.Trap: player.Trap(); break;
                            case Action.Crash: player.Crash(); break;
                            case Action.Break_Game: player.BreakGame(); break;
                            case Action.Destroy_Player:
                                if (player.AnticheatUser)
                                    PhotonNetwork.NetworkingClient.OpRaiseEvent(204, new Hashtable()
                                    {
                                        [0] = player.GameCharacter.photonView.ViewID
                                    }, new Photon.Realtime.RaiseEventOptions()
                                    {
                                        Receivers = Photon.Realtime.ReceiverGroup.All,
                                        TargetActors = player.PhotonPlayer.ActorNumber.SingleArray()
                                    }, SendOptions.SendReliable);
                                break;
                        }
                }
        }
        private void CheatCheck(CheatPlayer player, Hashtable props)
        {
            if (_blacklistedProps.Any(str => props.TryGetValue(str, out var val) && val is object obj && obj.ToString() != ForcedPropertyKey, out var val))
            {
                bool atlas = val.Contains("Atl");
                var flag = atlas ? CheatPlayer.PlayerFlag.Atlas : CheatPlayer.PlayerFlag.Cherry;

                if (!player.HasFlag(flag))
                {
                    player.AddFlag(flag);
                    LogUtil.Log($"Found [{(atlas? "Atlas": "Cherry")}] User: {player.Name}");
                }
            }
        }
    }
}