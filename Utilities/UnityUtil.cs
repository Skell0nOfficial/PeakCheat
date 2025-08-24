using PeakCheat.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace PeakCheat.Utilities
{
    public static class UnityUtil
    {
        private struct LineConstructor
        {
            public LineConstructor(int c, float w, Vector2 s)
            {
                count = c;
                width = w;
                size = s;
            }
            public int count;
            public float width;
            public Vector2 size;
        }
        private static int _previousFPS = int.MinValue;
        private static int GetFPS => (int)(1.0f / Time.deltaTime);
        private static Vector2 _mousePos = Vector2.zero;
        private static Color[] _colors = Array.Empty<Color>();
        private static readonly Vector3[] _directions = new Vector3[]
        {
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back,
            Vector3.up,
            Vector3.down
        };
        private static Dictionary<string, Texture2D> _cachedTextures = new Dictionary<string, Texture2D>();
        private static Dictionary<LineConstructor, Vector2[]> _linePositions = new Dictionary<LineConstructor, Vector2[]>();
        private static Dictionary<string, GUIStyle> _styles = new Dictionary<string, GUIStyle>();
        public static void ForEachStates(this GUIStyle style, Action<GUIStyleState, bool> method)
        {
            method(style.onActive, true);
            method(style.active, true);
            method(style.onFocused, true);
            method(style.focused, true);
            method(style.onHover, false);
            method(style.hover, false);
            method(style.onNormal, false);
            method(style.normal, false);
        }
        public static bool OnGround() => CheatUtil.CurrentScene >= SceneType.Airport? CheatPlayer.LocalPlayer?.OnGround ?? false: false;
        public static int FPS()
        {
            if (TimeUtil.CheckTime(.2f) && (Mathf.Abs(_previousFPS - GetFPS) >= 4)) _previousFPS = GetFPS;
            return _previousFPS;
        }
        public static string WithColor(this string str, Color c) => $"<color=#{ColorUtility.ToHtmlStringRGBA(c)}>{str}</color>";
        public static string Bold(this string str) => $"<b>{str}</b>";
        public static string Bold(this string str, int size) => Size(Bold(str), size);
        public static string Size(this string str, int size) => $"<size={Mathf.RoundToInt(ScreenSize().magnitude / 22030 * (size * 10f))}>{str}</b>";
        public static Vector3[] GetDirections()
        {
            var result = new List<Vector3>();
            foreach (var dir1 in _directions)
                foreach (var dir2 in _directions)
                {
                    if (dir1 == dir2) continue;
                    var newDir = dir1 + dir2;
                    if (newDir == Vector3.zero) continue;
                    result.AddIfNew(newDir);
                }
            return result.ToArray();
        }
        public static Vector3[] TimeShuffle(Vector3[] values, int seconds)
        {
            var result = (Vector3[])values.Clone();
            var random = new System.Random((Time.time / seconds).GetHashCode());

            for (int n = result.Length - 1; n > 0; n--)
            {
                int k = random.Next(n + 1);
                Vector3 temp = result[k];

                result[k] = result[n];
                result[n] = temp;
            }

            return result;
        }
        public static Vector3 ModifyDirection(this Vector3 dir, Vector3 pos, float distance)
        {
            bool CheckHit(Vector3 direction) => Physics.Raycast(pos, direction.normalized, distance);
            if (CheckHit(dir))
            {
                foreach (var direction in GetDirections())
                    if (!CheckHit(direction))
                        return direction;
                return Vector3.zero;
            }
            return dir;
        }
        public static Vector3 RandomDirection() => GetDirections().PickRandom();
        public static Vector3 WithX(this Vector3 vector, float x) => new Vector3(x, vector.y, vector.z);
        public static Vector3 WithY(this Vector3 vector, float y) => new Vector3(vector.x, y, vector.z);
        public static Vector3 WithZ(this Vector3 vector, float z) => new Vector3(vector.x, vector.y, z);
        public static Color WithRed(this Color c, float r) => new Color(r, c.g, c.b, c.a);
        public static Color WithGreen(this Color c, float g) => new Color(c.r, g, c.b, c.a);
        public static Color WithBlue(this Color c, float b) => new Color(c.r, c.g, b, c.a);
        public static Color WithAlpha(this Color c, float a) => new Color(c.r, c.g, c.b, Mathf.Clamp01(a));
        public static Color HideWhite(Color C, Color value) => C.Brightness(false) > .3f ? Color.clear : value;
        public static Color[] EveryColor()
        {
            if (_colors.Length > 0) return _colors;

            int num = 0;
            int steps = 11;

            _colors = new Color[steps * steps * steps * steps];

            for (int red = 0; red < steps; red++)
                for (int green = 0; green < steps; green++)
                    for (int blue = 0; blue < steps; blue++)
                        for (int alpha = 0; alpha < steps; alpha++)
                            _colors[num++] = new Color(red * .1f, green * .1f, blue * .1f, alpha * .1f );

            return _colors;

        }
        public static float Brightness(this Color c, bool withAlpha) => (c.r + c.g + c.b + (withAlpha? c.a: 0f)) / (withAlpha? 4f: 3f);
        public static GUIStyle CreateStyle(this GUIStyle original, Color color) => CreateStyle(original, color, color);
        public static GUIStyle CreateStyle(this GUIStyle original, Color normal, Color active)
        {
            string key = $"{original.name}:{normal}:{active}";
            if (!_styles.TryGetValue(key, out var style))
            {
                style = new GUIStyle(original);
                ForEachStates(style, (Style, Active) => Style.background = CreateTexture(Active ? active : normal, false, false));
                style.richText = true;
                _styles[key] = style;
            }

            return style;
        }
        public static Texture2D CreateTexture(this Color c, bool rounded) => CreateTexture(c, rounded, true);
        public static Texture2D CreateTexture(this Color c, bool rounded, bool tenRound)
        {
            if (tenRound)
            {
                c.r = TenRound(c.r);
                c.g = TenRound(c.g);
                c.b = TenRound(c.b);
                c.a = TenRound(c.a);
            }

            string key = c.ToString() + rounded.ToString();
            if (_cachedTextures.TryGetValue(key, out var texture)) return texture;
            
            if (rounded) texture = CreateTexture(Files.ButtonTexture, Vector2.one * 250, C => HideWhite(C, c));
            else
            {
                texture = new Texture2D(1, 1);
                texture.SetPixel(1, 1, c);
            }

            texture.Apply();
            _cachedTextures[key] = texture;
            return texture;
        }
        public static Texture2D CreateTexture(this Color c) => CreateTexture(c, false);
        public static Texture2D CreateTexture(byte[] bytes, Vector2 size, Func<Color, Color> replacer)
        {
            var texture = new Texture2D((int)size.x, (int)size.y);
            texture.LoadImage(bytes);
            var pixels = texture.GetPixels();
            var newColors = new Color[pixels.Length];

            for (int i = 0; i < pixels.Length; i++)
                newColors[i] = replacer(pixels[i]);

            texture.SetPixels(newColors);
            texture.Apply();
            return texture;
        }
        public static Texture2D CreateRoundedTexture(this Color c) => CreateTexture(c, true);
        public static Texture2D Flip(this Texture2D original)
        {
            int width = original.width;
            int height = original.height;
            var pixels = original.GetPixels();
            var newPixels = new Color[pixels.Length];
            var result = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    newPixels[y * width + x] = pixels[y * width + (width - x - 1)];

            result.SetPixels(newPixels);
            result.Apply();
            return result;
        }
        public static List<Transform> GetDescendants(this Transform parent)
        {
            List<Transform> descendants = new List<Transform>();
            foreach (Transform child in parent)
            {
                descendants.Add(child);
                descendants.AddRange(child.GetDescendants());
            }
            return descendants;
        }
        public static Vector3 CurrentPosition() => Character.localCharacter.Center;
        public static Vector2[] GenerateLinePositions(int buttonCount, float spacing, float lineWidth, Vector2 size)
        {
            var construct = new LineConstructor(buttonCount, lineWidth, size);
            if (_linePositions.TryGetValue(construct, out var vectors)) return vectors;

            int row = 0;
            float currentWidth = 0f;
            List<Vector2> list = new List<Vector2>();
            Vector2 space = size * (1f + spacing);

            for (int i = 0; i < buttonCount; i++)
            {
                if (currentWidth >= (lineWidth * (1f + spacing)))
                {
                    currentWidth = 0f;
                    row++;
                }

                list.Add(new Vector2(currentWidth, space.y * row));
                currentWidth += space.x;
            }

            var array = list.ToArray();
            _linePositions.Add(construct, array);
            return array;
        }
        public static float TenRound(float value) => Mathf.Round(value * 10f) / 10f;
        public static bool EvenTime() => TenRound(Time.time - (int)Time.time) % 2 == 0;
        public static float Bounce(float ping, float pong, float speed) =>
            Mathf.Lerp(ping, pong, Mathf.PingPong(Time.time * speed, 1f));
        public static Vector3 Abs(this Vector3 vector) => Modify(vector, Mathf.Abs);
        public static Vector3 Sign(this Vector3 vector) => Modify(vector, Mathf.Sign);
        public static Vector3 Cos(this Vector3 vector) => Modify(vector, Mathf.Cos);
        public static Vector3 Sin(this Vector3 vector) => Modify(vector, Mathf.Sin);
        public static Vector3 Asin(this Vector3 vector) => Modify(vector, Mathf.Asin);
        public static Vector3 Acos(this Vector3 vector) => Modify(vector, Mathf.Acos);
        public static Vector3 Atan(this Vector3 vector) => Modify(vector, Mathf.Atan);
        public static Vector3 Exp(this Vector3 vector) => Modify(vector, Mathf.Exp);
        public static Vector3 Round(this Vector3 vector) => Modify(vector, Mathf.Round);
        public static Vector3 Floor(this Vector3 vector) => Modify(vector, Mathf.Floor);
        public static Vector3 Ceil(this Vector3 vector) => Modify(vector, Mathf.Ceil);
        public static Vector3 Clamp01(this Vector3 vector) => Modify(vector, Mathf.Clamp01);
        public static Vector3 Modify(this Vector3 vector, Func<float, float> modifier)
        {
            return new Vector3()
            {
                x = modifier(vector.x),
                y = modifier(vector.y),
                z = modifier(vector.z)
            };
        }
        public static Vector3 Orbit(this Transform transform) => transform.position.Orbit();
        public static Vector3 Orbit(this Vector3 position) => position + OrbitVector();
        public static Vector3 OrbitVector() => OrbitVector(50f);
        public static Vector3 OrbitVector(float speed) => OrbitVector(speed, 1f);
        public static Vector3 OrbitVector(float speed, float distance) => (Quaternion.Euler(Vector3.up * (Time.time * speed)) * Vector3.forward).normalized * distance;
        public static void Lerp(this Transform current, Transform transform) => Lerp(current, transform, 4.7f);
        public static void Lerp(this Transform current, Transform transform, float speed)
        {
            current.position = Vector3.Lerp(current.position, transform.position, Time.deltaTime * speed);
            current.rotation = Quaternion.Lerp(current.rotation, transform.rotation, Time.deltaTime * speed);
        }
        public static Rect ScreenRect() => new Rect(Vector2.zero, ScreenSize());
        public static Vector2 ScreenSize() => new Vector2(Screen.width, Screen.height);
        public static Vector2 MiddleOfScreen() => ScreenSize() / 2f;
        public static Vector2 MousePos() => _mousePos = Vector2.Lerp(_mousePos, Event.current.mousePosition, Time.deltaTime * 7.4f);
        public static async Task<Texture2D?> CaptureImage(this Camera camera)
        {
            var x = Mathf.RoundToInt(Screen.width);
            var y = Mathf.RoundToInt(Screen.height);
            var render = RenderTexture.GetTemporary(x, y, 24, RenderTextureFormat.ARGB32);
            if (render == null) return null;
            camera.targetTexture = render;
            camera.Render();
            var format = TextureFormat.RGBA32;
            var request = AsyncGPUReadback.Request(render, 0, format);
            while (!request.done) await Task.Delay(1);
            camera.targetTexture = null;
            RenderTexture.ReleaseTemporary(render);
            render = null;
            if (request.hasError) return null;
            var data = request.GetData<byte>();
            if (data.Length == 0) return null;
            var texture = new Texture2D(x, y, format, false);
            texture.LoadRawTextureData(data);
            texture.Apply();
            return texture;
        }
    }
}