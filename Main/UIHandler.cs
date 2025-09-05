using PeakCheat.Patches;
using PeakCheat.Types;
using PeakCheat.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PeakCheat.Main
{
    public class UIHandler: MonoBehaviour, CheatBehaviour
    {
        private static bool _open = false;
        private static readonly List<UITab> _tabs = new List<UITab>();
        private static Texture2D? _background = null, _closeTab = null;
        private static GUIStyle? _selector = null, _tabBG = null, _tabRoof = null, _tabButton = null, _tabButtonEnabled = null;
        private static Vector2 _mousePos = Vector2.zero, _maxSize = Vector2.zero;
        private static Rect _selectorRect = Rect.zero;
        public static Texture2D Background => _background ??= GetTexture(TextureType.Background);
        public static Texture2D CloseTab => _closeTab ??= GetTexture(TextureType.CloseButton);
        public static GUIStyle Selector => _selector ??= GetStyle(StyleType.Selector);
        public static GUIStyle TabBG => _tabBG ??= GetStyle(StyleType.TabBG);
        public static GUIStyle TabRoof => _tabRoof ??= GetStyle(StyleType.TabRoof);
        public static GUIStyle TabButton => _tabButton ??= GetStyle(StyleType.TabButton);
        public static GUIStyle TabButtonEnabled => _tabButtonEnabled ??= GetStyle(StyleType.TabButtonEnabled);
        public static bool Open
        {
            get => _open && !Zorro.UI.Modal.Modal.IsOpen;
            set
            {
                if (_open == value) return;
                _open = value;
                AudioUtil.Click();
                if (_open) _mousePos = Event.current.mousePosition;
            }
        }
        public static bool GUIActive() => Open;
        public static void AddTab(UITab tab) => _tabs.AddIfNew(tab);
        private static Texture2D GetTexture(TextureType type)
        {
            return type switch
            {
                TextureType.Background => Color.black.WithAlpha(.8f).CreateTexture(),
                TextureType.CloseButton => UnityUtil.CreateTexture(Files.CloseTab, Vector2.one * 340f, C => C.WithAlpha(C.g)),
                _ => Texture2D.blackTexture
            };
        }
        private static GUIStyle GetStyle(StyleType type)
        {
            return type switch
            {
                StyleType.Selector => GUI.skin.window.CreateStyle(Color.black),
                StyleType.TabBG => GUI.skin.window.CreateStyle(Color.black),
                StyleType.TabRoof => GUI.skin.window.CreateStyle((Color.white * .017f).WithAlpha(1f)),
                StyleType.TabButton => GUI.skin.button.CreateStyle((Color.white * .02f).WithAlpha(.7f)),
                StyleType.TabButtonEnabled => GUI.skin.button.CreateStyle((Color.white * .025f).WithAlpha(1f)),
                _ => GUI.skin.window
            };
        }
        void CheatBehaviour.Start()
        {
            foreach (var color in UnityUtil.EveryColor()) color.CreateTexture();

            LogUtil.Log("Generated color textures");
        }
        void CheatBehaviour.Update()
        {
            _mousePos = Event.current.mousePosition;

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                Open = !Open;
                CursorSettings.ShowCursor(Open);
            }
        }
        public void Start()
        {
            int num = 0;
            var size = new Vector2(540f, 490f);
            var positions = UnityUtil.GenerateLinePositions(_tabs.Count, .1f, Screen.width * .9f, size);
            foreach (var tab in _tabs)
            {
                tab.Data.Position = positions[num] + Vector2.up * 25f;
                tab.Data.Width = size.x;
                tab.Data.Height = size.y;

                LogUtil.Log($"Initialized tab: {tab.Name} ({num++})");
            }
        }
        public void OnGUI()
        {
            if (!Open) return;

            if (_tabs.Any(T => T.Data.Dragging, out var dragTab))
                if (!Input.GetMouseButton(0)) dragTab.Data.Dragging = false;
                else dragTab.Data.Position = Event.current.mousePosition + dragTab.Data.DragOffset;

            foreach (var tab in _tabs)
            {
                var data = tab.Data;

                if (data.Open)
                {
                    var tabRect = data.GetRect;
                    GUI.Window(GeneralUtil.Compute(tab.Name), tabRect, I => tab.Render(), "", TabBG);
                    
                    var roofSize = new Vector2(tabRect.width, 30f);
                    var roofRect = new Rect(tabRect.position - Vector2.up * roofSize.y, roofSize);
                    GUI.Window(GeneralUtil.Compute(tab.Name + "Roof"), roofRect, I => RenderTabRoof(tab, roofRect), "", TabRoof);
                    
                    if (roofRect.Contains(_mousePos) && Input.GetMouseButtonDown(0) && !data.Dragging)
                    {
                        tab.Data.DragOffset = data.Position - _mousePos;
                        tab.Data.Dragging = true;
                    }
                }
            }

            GUI.DrawTexture(UnityUtil.ScreenRect(), Background);
            GUI.Window(GeneralUtil.Compute(Time.frameCount), _selectorRect, I => RenderSelector(), "", Selector);

            GUI.Label(new Rect(Vector2.zero, new Vector2(Screen.width / 2f, Screen.height / 8f)), "PeakCheat".Bold().Size(85));
        }
        private void RenderTabRoof(UITab tab, Rect rect)
        {
            GUI.Label(new Rect(Vector2.zero, new Vector2(500f, tab.Data.Height)), tab.Name.Bold().Size(20));
            
            var size = new Vector2(rect.width / 17f, rect.height);
            var closeRect = new Rect(new Vector2(rect.width - size.x, 0f), size);
            
            GUI.DrawTexture(closeRect, CloseTab);
            closeRect.position += rect.position;
            
            if (closeRect.Contains(_mousePos) && Input.GetMouseButtonDown(0))
            {
                AudioUtil.Click();
                tab.Data.Open = false;
            }
        }
        private void RenderSelector()
        {
            var h = Screen.height;
            var w = Screen.width;
            
            _maxSize = new Vector2(w - w / 3.7f, h / 24f);
            _selectorRect.center = new Vector2(w / 2f, h - _maxSize.y / 2f);

            var width = _maxSize.x / 13f;
            var height = _maxSize.y - _maxSize.y / 12f;
            var size = new Vector2(width, height);
            var maxTabs = Mathf.FloorToInt(_maxSize.x / width) * 5f;
            var tabCount = _tabs.Count;

            _selectorRect.size = _maxSize;
            for (int i = 0; i < tabCount; i++)
            {
                if (i > maxTabs) continue;

                var tab = _tabs.OrderBy(T => T.Order).ToArray()[i];
                var data = tab.Data;
                var rect = Rect.zero;

                rect.size = size;
                rect.position = new Vector2(10f + (width + 3f) * i, 10f);

                GUI.Button(rect, tab.Name.Size(15), data.Open ? TabButtonEnabled : TabButton);
                if (rect.Contains(Event.current.mousePosition) && Input.GetMouseButtonDown(0) && TimeUtil.CheckTime(.2f))
                {
                    tab.Data.Open = !tab.Data.Open;
                    tab.Toggle(tab.Data.Open);
                    AudioUtil.Click();
                }
            }
        }
    }
    public enum TextureType
    {
        Background,
        CloseButton
    };
    public enum StyleType
    {
        Selector,
        TabBG,
        TabRoof,
        TabButton,
        TabButtonEnabled
    };
}