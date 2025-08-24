using PeakCheat.Types;
using PeakCheat.Utilities;
using PeakCheat.Patches;

namespace PeakCheat.Cheats.Gameplay
{
    #pragma warning disable CS8601
    internal class Immortality: Cheat
    {
        public override string Name => "Immortality";
        public override string Description => "Grants you Immortality";
        public override SceneType RequiredScene => SceneType.Level;
        public override void Enable() => FaintPatch.PassOut += Unfaint;
        public override void Disable() => FaintPatch.PassOut -= Unfaint;
        public override void Method()
        {
            CheatPlayer.LocalPlayer.Revive();
            Character.localCharacter.data.fallSeconds = 0f;
            if (Character.localCharacter.refs.afflictions.statusSum != 0)
                CheatPlayer.LocalPlayer.ResetStatuses();
        }
        public static void Unfaint(Character character)
        {
            if (!character.IsLocal) return;
            PlayerUtil.PlayerRPC(character, "RPCA_UnPassOut");
        }
    }
    #pragma warning restore CS8601
}