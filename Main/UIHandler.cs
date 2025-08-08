using PeakCheat.Classes;
using PeakCheat.Utilities;
using UnityEngine;

namespace PeakCheat.Main
{
    public class UIHandler: MonoBehaviour, CheatBehaviour
    {
        private void OnGUI()
        {
            GUI.Button(new Rect(Event.current.mousePosition + (Vector2.one * -83f), new Vector2(120f, 80f)), "<b>Tester</b>", UnityUtil.GetButton(Color.black, (Color.white * .1f).WithAlpha(1f)));
        }
    }
}