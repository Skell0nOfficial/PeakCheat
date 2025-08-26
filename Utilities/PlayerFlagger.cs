using ExitGames.Client.Photon;
using PeakCheat.Types;
using Photon.Pun;

namespace PeakCheat.Utilities
{
    internal class PlayerFlagger: EventBehaviour, CheatBehaviour
    {
        private static readonly string[] _blacklistedProps = new string[]
        {
            "AtlUser",
            "AtlOwner",
            "CherryUser",
            "CherryOwner"
        };
        public const string ForcedPropertyKey = "PeakCheat::ForcedProperty";
        bool CheatBehaviour.DelayStart() => true;
        void CheatBehaviour.Start() => PhotonCallbacks.PropertiesUpdate += CheatCheck;
        void EventBehaviour.OnEvent(byte code, int sender, object data)
        {
            if (!PhotonNetwork.InRoom) return;

            if (code == 69 && PhotonNetwork.TryGetPlayer(sender, out var p) && p.ToCheatPlayer() is CheatPlayer player && !player.HasFlag(CheatPlayer.PlayerFlag.Anticheat))
            {
                LogUtil.Log($"Found Anticheat User: {player.Name}");
                player.AddFlag(CheatPlayer.PlayerFlag.Anticheat);
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