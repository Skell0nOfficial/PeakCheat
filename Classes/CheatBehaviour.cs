using UnityEngine;

namespace PeakCheat.Classes
{
    public interface CheatBehaviour
    {
        public void Start() { }
        public void OnLoad() { }
        public void Update() {}
        public void RenderUI() {}
    }
}