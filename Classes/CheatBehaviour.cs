using UnityEngine;

namespace PeakCheat.Classes
{
    public class CheatBehaviour
    {
        public CheatBehaviour()
        {
            Debug.Log($"[{GetType().Name}] Loaded Successfully");
        }
        public virtual void Start() { }
        public virtual void OnLoad() { }
        public virtual void Update() {}
        public virtual void RenderUI() {}
    }
}