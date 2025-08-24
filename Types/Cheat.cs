namespace PeakCheat.Types
{
    public abstract class Cheat
    {
        public virtual string Name => GetType()?.Name?? "Untitled Cheat";
        public virtual string Description => "Empty Description";
        public virtual SceneType RequiredScene => SceneType.Unknown;
        public virtual void Method() {}
        public virtual void Enable() {}
        public virtual void Disable() {}
        public virtual void Init() {}
        public virtual bool Hide() => false;
        public virtual bool DefaultEnabled => false;
        public int GetID() => (Name + Description).GetHashCode();
        private bool _enabled = false;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
            }
        }
    }
}