using PeakCheat.Classes;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        private static Dictionary<LineConstructor, Vector2[]> _linePositions = new Dictionary<LineConstructor, Vector2[]>();
        private static Dictionary<Color, Texture2D> _cachedTextures = new Dictionary<Color, Texture2D>();
        private static Dictionary<KeyValuePair<Color, Color>, GUIStyle> _styles = new Dictionary<KeyValuePair<Color, Color>, GUIStyle>();
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
        public static bool OnGround() => CheatPlayer.LocalPlayer.OnGround;
        public static Vector3 WithX(this Vector3 vector, float x) => new Vector3(x, vector.y, vector.z);
        public static Vector3 WithY(this Vector3 vector, float y) => new Vector3(vector.x, y, vector.z);
        public static Vector3 WithZ(this Vector3 vector, float z) => new Vector3(vector.x, vector.y, z);
        public static Color WithA(this Color c, float a) => new Color(c.r, c.g, c.b, a);
        public static GUIStyle GetButton(Color normal, Color active)
        {
            var pair = new KeyValuePair<Color, Color>(normal, active);
            if (!_styles.TryGetValue(pair, out var style))
            {
                var n = FromColor(normal);
                var a = FromColor(active);
                style = new GUIStyle(GUI.skin.button);
                ForEachStates(style, (Style, Active) => Style.background = Active ? a : n);
                style.richText = true;
            }

            return style;
        }
        public static Texture2D FromColor(this Color c)
        {
            if (_cachedTextures.TryGetValue(c, out var texture)) return texture;

            texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, c);
            texture.Apply();

            _cachedTextures.Add(c, texture);
            return texture;
        }
        public static Vector3 CurrentPosition() => Character.localCharacter.Center;
        public static Vector2[] GenerateLinePositions(int buttonCount, float lineWidth, Vector2 size)
        {
            var construct = new LineConstructor(buttonCount, lineWidth, size);
            if (_linePositions.TryGetValue(construct, out var vectors)) return vectors;
            int row = 0;
            float currentWidth = 0f;
            List<Vector2> list = new List<Vector2>();
            Vector2 spacing = size.normalized * (size.magnitude * 1.25f);

            for (int i = 0; i < buttonCount; i++)
            {
                if (currentWidth >= lineWidth)
                {
                    currentWidth = 0f;
                    row++;
                }

                list.Add(new Vector2(currentWidth, spacing.y * row));
                currentWidth += size.x;
            }

            var array = list.ToArray();
            _linePositions.Add(construct, array);
            return array;
        }
        public static bool EvenTime() => ((int)((Time.time - (int)Time.time) * 10)) % 2 == 0;
        public static float Bounce(float ping, float pong, float speed) =>
            Mathf.Lerp(ping, pong, Mathf.PingPong(Time.time * speed, 1f));
        public static Vector3 Orbit(this Transform transform) => transform.position.Orbit();
        public static Vector3 Orbit(this Vector3 position) => position + OrbitVector();
        public static Vector3 OrbitVector() => OrbitVector(50f);
        public static Vector3 OrbitVector(float speed) => OrbitVector(speed, 1f);
        public static Vector3 OrbitVector(float speed, float distance)
        {
            Vector3 rotationVector = Vector3.up * (Time.time * speed);
            Vector3 forwardVector = Quaternion.Euler(rotationVector) * Vector3.forward;
            return forwardVector.normalized * distance;
        }
        public static void Lerp(this Transform current, Transform transform) => Lerp(current, transform, 4.7f);
        public static void Lerp(this Transform current, Transform transform, float speed)
        {
            current.position = Vector3.Lerp(current.position, transform.position, Time.deltaTime * speed);
            current.rotation = Quaternion.Lerp(current.rotation, transform.rotation, Time.deltaTime * speed);
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
    }
}