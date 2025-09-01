using System.Collections.Generic;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public static class TimeUtil
    {
        private static Dictionary<string, float> _times = new Dictionary<string, float>();
        public static bool CheckTime(float delay) => CheckTime(GeneralUtil.GetFrame().Signature(), delay);
        public static bool CheckTime(string key, float delay)
        {
            if (!_times.TryGetValue(key, out var value)) value = 0f;
            if (Time.time >= (value * Time.timeScale))
            {
                _times[key] = Time.time + delay;
                return true;
            }
            return false;
        }
        public static bool CheckTime(string key)
        {
            if (!_times.TryGetValue(key, out var value)) value = 0f;
            return Time.time >= (value * Time.timeScale);
        }
        public static void SetTime(float delay) => _times[GeneralUtil.GetFrame().Signature()] = Time.time + delay;
        public static void SetTime(string key, float delay) => _times[key] = Time.time + delay;
    }
}
