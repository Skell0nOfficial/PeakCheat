using PeakCheat.Utilities;
using Photon.Pun;
using UnityEngine;

namespace PeakCheat.Types
{
    #pragma warning disable CS8618
    public class CheatPlayer
    {
        public CheatPlayer(Player player)
        {
            if (player == null) return;
            var playerCharacter = player.character;
            if (!player.TryGetPhotonPlayer(out var photonPlayer) || !(photonPlayer is Photon.Realtime.Player p)) return;
            View = playerCharacter.photonView;
            PhotonPlayer = p;
            GamePlayer = player;
            GameCharacter = playerCharacter;
        }
        public string Name => GameCharacter?.characterName ?? "null";
        public string UserId => PhotonPlayer?.UserId ?? "null";
        public PhotonView View;
        public global::Player GamePlayer;
        public Character GameCharacter;
        public Photon.Realtime.Player PhotonPlayer;
        public CharacterData? CharacterData => GameCharacter?.data;
        public Transform? HeadTransform => GameCharacter.refs.head?.transform;
        public Transform? BodyTransform => GameCharacter.refs.hip?.transform;
        public Color PlayerColor => GameCharacter?.refs.customization?.PlayerColor?? Color.black;
        public Vector3 Position => GameCharacter?.Center?? Vector3.zero;
        public bool OnGround => CharacterData?.isGrounded?? false;
        public bool Alive => !Dead;
        public bool Dead => CharacterData?.dead ?? false;
        public bool IsLocal => GameCharacter?.IsLocal ?? PhotonPlayer?.IsLocal ?? false;
        public bool PassedOut => CharacterData?.passedOut?? false;
        public int? Actor => PhotonPlayer?.ActorNumber;
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
        public override string ToString() => $"[{Name}] ({GetType().Name})";
        public static implicit operator CheatPlayer(Photon.Realtime.Player player) => player.ToCheatPlayer();
        public static implicit operator CheatPlayer(global::Player player) => player.ToCheatPlayer();
        public static implicit operator CheatPlayer(Character character) => character.ToCheatPlayer();
        public static implicit operator global::Player(CheatPlayer player) => player.GamePlayer;
        public static implicit operator Character(CheatPlayer player) => player.GameCharacter;
        public static implicit operator Vector3(CheatPlayer player) => player.Position;
        public static implicit operator Photon.Realtime.Player(CheatPlayer player) => player.PhotonPlayer;
        public static implicit operator PhotonView(CheatPlayer player) => player.View;
    }
    #pragma warning restore CS8618
}