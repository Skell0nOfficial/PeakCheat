using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public static class GeneralUtil
    {
        public static async void DelayInvoke(this Action action, float delay)
        {
            await Task.Delay(Mathf.RoundToInt(delay * 1000f));
            action();
        }
        public static T[] SingleArray<T>(this T obj) => new T[] { obj };
        public static T[] SingleList<T>(this T obj) => SingleArray(obj);
        public static string Signature(this StackFrame frame)
        {
            var method = frame.GetMethod();
            var type = method.DeclaringType;
            var args = method.GetParameters();

            return string.Join("\n", new object[]
            {
                method.Name,
                type.Name,
                type.Namespace,
                args.Length,
                string.Join('.', args.Select(a => a.ParameterType.Name))
            });
        }
        public static int Compute(string s)
        {
            if (s == null || s.Length == 0) return 0;

            int length = s.Length;
            uint num = (uint)length;
            length >>= 1;
            int num3 = 0;
            while (length > 0)
            {
                num += s[num3];
                uint num4 = ((uint)s[num3 + 1] << 11) ^ num;
                num = (num << 16) ^ num4;
                num3 += 2;
                num += num >> 11;
                length--;
            }

            if ((length & 1) == 1)
            {
                num += s[num3];
                num ^= num << 11;
                num += num >> 17;
            }

            num ^= num << 3;
            num += num >> 5;
            num ^= num << 4;
            num += num >> 17;
            num ^= num << 25;
            return (int)(num + (num >> 6));
        }
        public static StackFrame GetFrame() => new StackTrace().GetFrame(2);
    }
}