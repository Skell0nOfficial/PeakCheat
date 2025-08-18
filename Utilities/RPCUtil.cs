using PeakCheat.Types;

namespace PeakCheat.Utilities
{
    internal class RPCUtil: CheatBehaviour
    {
        void T() => PeakCheat.Utilities.PlayerUtil.Crash(PlayerHandler.GetAllPlayerCharacters().Find(C => C.characterName.Contains("Kris")));
    }
}