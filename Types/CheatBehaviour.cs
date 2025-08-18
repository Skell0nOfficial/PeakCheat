namespace PeakCheat.Types
{
    public interface CheatBehaviour
    {
        public bool DelayStart() => false;
        public void Start() {}
        public void OnLoad() {}
        public void Update() {}
    }
}