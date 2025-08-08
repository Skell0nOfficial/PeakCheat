using PeakCheat.Classes;

namespace PeakCheat.Utilities
{
    internal class ACDisabler: EventBehaviour
    {
        private string Name => GetType().Name;
        void EventBehaviour.OnEvent(byte code, int sender, object data)
        {
            LogUtil.Log($"[{Name}] Got event: (CODE: {code}, SENDER: {sender}, OBJ: {data.GetType().Name})");
            if (code == 69)
            {
                LogUtil.Log($"[{Name}] Got AntiCheat event from: {sender}");
            }
        }
    }
}
