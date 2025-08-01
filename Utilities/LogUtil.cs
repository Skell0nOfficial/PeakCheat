using BepInEx.Logging;
using PeakCheat.Classes;
using System;
using System.Collections.Generic;

namespace PeakCheat.Utilities
{
    public class LogUtil: CheatBehaviour
    {
        public enum LogLevel { None, Warning, Error }
        private static Dictionary<LogLevel, ManualLogSource> _loggers = new Dictionary<LogLevel, ManualLogSource>();
        void CheatBehaviour.Start()
        {
            if (_loggers.Count != 0) return;
            foreach (var level in Enum.GetValues(typeof(LogLevel)))
                _loggers.Add((LogLevel)level, Logger.CreateLogSource($"PeakCheat:{level.ToString()}"));
        }
        public static void Log(string message) => Log(0, message);
        public static void Log(bool error, string message) => Log(error? 2: 1, message);
        public static void Log(int level, string message) => Log((LogLevel)level, message);
        public static void Log(LogLevel level, string message)
        {
            if (!_loggers.TryGetValue(level, out var log)) return;
            log.Log(Convert(level), message);
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