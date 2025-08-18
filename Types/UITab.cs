using PeakCheat.Main;
using UnityEngine;

namespace PeakCheat.Types
{
    public abstract class UITab
    {
        public UITab()
        {
            UIHandler.AddTab(this);
            Data = new TabData()
            {
                Width = 180f,
                Height = 120f,
                Position = Vector2.zero
            };
        }
        public TabData Data;
        public virtual string Name => "Untitled Tab";
        public virtual Color BGColor => Color.black;
        public virtual void Render() {}
    }
}