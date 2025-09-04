using PeakCheat.Main;
using PeakCheat.Utilities;
using System;

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
        public int GetID() => GeneralUtil.Compute(Name + Description);
        private bool _enabled = false;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                CheatHandler.SaveCheats();
            }
        }
        private readonly object[] _data = new object[short.MaxValue];
        public bool GetData<T>(int index, out object? obj)
        {
            if (index >= _data.Length)
            {
                obj = null;
                return false;
            }

            obj = _data[index];
            return true;
        }
        public void SetData(int index, object obj) => _data[index] = obj;
    }
}