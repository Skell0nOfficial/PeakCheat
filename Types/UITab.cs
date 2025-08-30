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
        public virtual int Order => -1;
        public virtual void Render() {}
        public virtual void Toggle(bool toggled) {}
    }
}