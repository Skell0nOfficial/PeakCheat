using UnityEngine;

namespace PeakCheat.Classes
{
    public abstract class UITab: CheatBehaviour
    {
        public abstract string Name { get; }
        public virtual Color BGColor => Color.black;
        public virtual void Render()
        {

        }
    }
}
