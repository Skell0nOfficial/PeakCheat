using System;
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
    }
}
