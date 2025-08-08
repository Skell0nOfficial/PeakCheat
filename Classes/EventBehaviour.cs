namespace PeakCheat.Classes
{
    public interface EventBehaviour: CheatBehaviour
    {
        public void OnEvent(byte code, int sender, object data) {}
    }
}