using HarmonyLib;
using PeakCheat.Utilities;
using System;
using UnityEngine;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(Logger), nameof(Logger.Log), new Type[] { typeof(LogType), typeof(object) })]
    internal class BetterLogging
    {
        static void Postfix(LogType logType, object message) => LogUtil.Register($"[{logType}] {message}");
    }
}