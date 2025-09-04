using HarmonyLib;
using PeakCheat.Types;
using PeakCheat.Utilities;
using System.Linq;
using TMPro;
using PhotonPlayer = Photon.Realtime.Player;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(UIPlayerNames), nameof(UIPlayerNames.UpdateName))]
    class NametagPatch
    {
        static void Prefix(UIPlayerNames __instance, int index)
        {
            if (__instance.playerNameText is PlayerName[] names &&
                index < names.Length
                && names[index]?.text is TextMeshProUGUI text
                && names[index]?.characterInteractable?.character is Character character
                && character?.photonView?.Owner is PhotonPlayer player) UpdateText(text, character, player);
        }
        static void UpdateText(TextMeshProUGUI text, CheatPlayer player, PhotonPlayer creator)
        {
            var str = string.Empty;

            str += player.GameCharacter?.characterName?? "null";

            if (creator.NickName.ToLower().Trim() != str.ToLower().Trim()) str += $" ({creator.NickName})"; str += $" [{creator.ActorNumber}]";
            if (player.Flags.Length > 0) str += $"\n[{string.Join(", ", player.Flags.Select(F => F.ToString()))}]";

            text.richText = true;
            text.fontStyle = FontStyles.Italic;
            text.text = str.WithColor(player.PlayerColor);
        }
    }
}
