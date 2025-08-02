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
                frame.GetFileLineNumber(),
                method.Name,
                type.Name,
                type.Namespace,
                args.Length,
                string.Join('.', args.Select(a => a.Name))
            });
        }
    }
}