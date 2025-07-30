using PeakCheat.Utilities;
using Photon.Pun;

namespace PeakCheat.Classes
{
    #pragma warning disable CS8618
    public class CheatPlayer
    {
        public CheatPlayer(Player player)
        {
            if (player == null) return;
            var playerCharacter = player.character;
            if (!player.TryGetPhotonPlayer(out var photonPlayer)) return;
            Name = playerCharacter.characterName;
            UserId = photonPlayer.UserId;
            View = playerCharacter.photonView;
            PhotonPlayer = photonPlayer;
            GamePlayer = player;
            GameCharacter = player.character;
        }
        public string Name = string.Empty;
        public string UserId = string.Empty;
        public PhotonView View;
        public global::Player GamePlayer;
        public global::Character GameCharacter;
        public Photon.Realtime.Player PhotonPlayer;
        public static CheatPlayer LocalPlayer => Character.localCharacter.ToCheatPlayer();
        public static implicit operator CheatPlayer(global::Player player) => new CheatPlayer(player);
        public static implicit operator CheatPlayer(Photon.Realtime.Player player) => new CheatPlayer(PlayerHandler.GetPlayer(player));
        public static implicit operator CheatPlayer(Character character) => new CheatPlayer(character.player);
        public static implicit operator global::Player(CheatPlayer player) => player.GamePlayer;
        public static implicit operator Photon.Realtime.Player(CheatPlayer player) => player.PhotonPlayer;
        public static implicit operator PhotonView(CheatPlayer player) => player.View;
    }
    #pragma warning restore CS8618
}
