using PeakCheat.Utilities;
using Photon.Pun;
using System.Linq;

namespace PeakCheat.Main
{
    public class Promotion
    {
        public const string DiscordLink = "discord.gg/Nn2KHseZ5";
        public static string PromotionString => string.Join('\n', new int[300].Select(I => string.Join("     ", new int[20].Select(I => $"PeakCheat: {DiscordLink}"))));
        public static void SpamInvitation()
        {
            if (PhotonNetwork.InRoom)
                foreach (var player in PhotonNetwork.PlayerList)
                    Exploits.ForceSetName(player.ActorNumber, PromotionString[..(short.MaxValue / 3)]);
        }
    }
}