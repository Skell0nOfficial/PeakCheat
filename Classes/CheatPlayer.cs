using PeakCheat.Utilities;
using Photon.Pun;
using UnityEngine;

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
        public Character GameCharacter;
        public Photon.Realtime.Player PhotonPlayer;
        public CharacterData? CharacterData => GameCharacter?.data;
        public Transform? HeadTransform => GameCharacter.refs.ragdoll.partDict[BodypartType.Head]?.transform;
        public Transform? BodyTransform => GameCharacter.refs.ragdoll.partDict[BodypartType.Hip]?.transform;
        public Vector3 Position => GameCharacter?.Center?? Vector3.zero;
        public bool OnGround => CharacterData?.isGrounded?? true;
        public bool Alive => !Dead;
        public bool IsLocal => GameCharacter?.IsLocal?? PhotonPlayer?.IsLocal?? false;
        public bool Dead => CharacterData?.dead ?? false;
        public bool GetItem(out Item? item)
        {
            var currentItem = CharacterData?.currentItem;

            if (currentItem != null)
            {
                item = currentItem;
                return true;
            }

            item = null;
            return false;
        }
        public static CheatPlayer LocalPlayer => Character.localCharacter.ToCheatPlayer();
        public static implicit operator CheatPlayer(global::Player player) => player.ToCheatPlayer();
        public static implicit operator CheatPlayer(Photon.Realtime.Player player) => player.ToCheatPlayer();
        public static implicit operator CheatPlayer(Character character) => character.ToCheatPlayer();
        public static implicit operator global::Player(CheatPlayer player) => player.GamePlayer;
        public static implicit operator Photon.Realtime.Player(CheatPlayer player) => player.PhotonPlayer;
        public static implicit operator PhotonView(CheatPlayer player) => player.View;
    }
    #pragma warning restore CS8618
}