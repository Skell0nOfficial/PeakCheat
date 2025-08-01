using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public static class TimeUtil
    {
        private static Dictionary<string, float> _times = new Dictionary<string, float>();
        private static StackFrame GetFrame => new StackTrace().GetFrame(2);
        public static bool CheckTime(float delay) => CheckTime(GetFrame.Signature(), delay);
        public static bool CheckTime(string key, float delay)
        {
            if (!_times.TryGetValue(key, out var value)) value = 0f;
            if (Time.time >= value)
            {
                _times[key] = Time.time + delay;
                return true;
            }
            return false;
        }
    }
}
