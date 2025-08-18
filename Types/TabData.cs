using UnityEngine;

namespace PeakCheat.Types
{
    public struct TabData
    {
        public bool Open, Sizing, Dragging;
        public float Width, Height;
        public Vector2 Position, DragOffset;
        public Vector2 Size => new Vector2(Width, Height);
    }
}