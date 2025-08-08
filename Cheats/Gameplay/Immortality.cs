using PeakCheat.Classes;
using PeakCheat.Patches;
using PeakCheat.Utilities;

namespace PeakCheat.Cheats.Gameplay
{
    #pragma warning disable CS8601
    internal class Immortality: Cheat
    {
        public override string Name => "Immortality";
        public override string Description => "Grants you Immortality";
        public override void Enable() => FaintPatch.PassOut += Unfaint;
        public override void Disable() => FaintPatch.PassOut -= Unfaint;
        public override void Method()
        {
            CheatPlayer.LocalPlayer.Revive();
            Character.localCharacter.data.fallSeconds = 0f;
        }
        public static void Unfaint(Character character)
        {
            if (!character.IsLocal) return;
            PlayerUtil.PlayerRPC(character, "RPCA_UnPassOut");
        }
    }
    #pragma warning restore CS8601
}