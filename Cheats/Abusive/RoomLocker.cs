using PeakCheat.Types;
using Photon.Pun;

namespace PeakCheat.Cheats.Abusive
{
    internal class RoomLocker: Cheat
    {
        public override string Name => $"{(Locked? "Unlock": "Lock")} Room";
        public override string Description => $"{(Locked ? "Unlock" : "Lock")}s your current room";
        public static bool Locked => PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.MaxPlayers == 0;

    }
}