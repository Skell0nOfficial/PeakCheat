using BepInEx.Logging;
using HarmonyLib;
using PeakCheat.Types;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public class LogUtil: CheatBehaviour
    {
        public enum LogLevel { None, Warning, Error }
        private static ManualLogSource? _logger = null;
        void CheatBehaviour.Start() => _logger = BepInEx.Logging.Logger.CreateLogSource("PeakCheat");
        public static void Log(string message) => Log(0, message);
        public static void Log(bool error, string message) => Log(error? 2: 1, message);
        public static void Log(int level, string message) => Log((LogLevel)level, message);
        public static void Log(LogLevel level, string message)
        {
            if (!(_logger is ManualLogSource Logger))
            {
                Debug.LogWarning("[PeakCheat] Logger returned null");
                return;
            }

            if (Object.FindFirstObjectByType<PlayerConnectionLog>() is PlayerConnectionLog LoggerObject)
                Traverse.Create(LoggerObject)
                    .Method("AddMessage", "PeakCheat")
                    .GetValue($"<color=#{ConvertHex(level)}>[PeakCheat] {message}</color>");

            Logger.Log(Convert(level), message);
        }
        public static string ConvertHex(LogLevel level)
        {
            return level switch
            {
                LogLevel.None => "808080",
                LogLevel.Warning => "c1e620",
                LogLevel.Error => "ba0d0d",
                _ => "000000"
            };
        }
        public static BepInEx.Logging.LogLevel Convert(LogLevel level)
        {
            return level switch
            {
                LogLevel.None => BepInEx.Logging.LogLevel.Info,
                LogLevel.Warning => BepInEx.Logging.LogLevel.Warning,
                LogLevel.Error => BepInEx.Logging.LogLevel.Error,
                _ => BepInEx.Logging.LogLevel.None
            };
        }
    }
}