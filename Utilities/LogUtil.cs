using BepInEx.Logging;
using PeakCheat.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public class LogUtil: CheatBehaviour
    {
        public enum LogLevel { None, Warning, Error }
        static ManualLogSource? _logger = null;
        static bool _blockNotification = false;
        static readonly List<string> _currentLog = new List<string>();
        public static string[] GetLogEntries() => _currentLog.ToArray().Reverse().ToArray();
        public static string Information
        {
            set
            {
                _blockNotification = true;
                Log(value);
                _blockNotification = false;
            }
        }
        public static string Warning
        {
            set
            {
                _blockNotification = true;
                Log(false, value);
                _blockNotification = false;
            }
        }
        public static string Error
        {
            set
            {
                _blockNotification = true;
                Log(true, value);
                _blockNotification = false;
            }
        }
        void CheatBehaviour.Start() => _logger = BepInEx.Logging.Logger.CreateLogSource("PeakCheat");
        public static void Register(string log)
        {
            _currentLog.Add(log);
            GeneralUtil.DelayInvoke(() => _currentLog.RemoveAt(0), 7f);
        }
        public static void Log(object message) => Log(0, message);
        public static void Log(bool error, object message) => Log(error? 2: 1, message);
        public static void Log(int level, object message) => Log((LogLevel)level, message);
        public static void Log(LogLevel level, object message)
        {
            if (!(_logger is ManualLogSource Logger)) return;

            if (!_blockNotification)
            {
                if (Object.FindFirstObjectByType<PlayerConnectionLog>() is PlayerConnectionLog LoggerObject)
                    LoggerObject.AddMessage($"<color=#{ConvertHex(level)}>[PeakCheat] {message}</color>");
                Register($"[{level}] {message}");
            }

            Logger.Log(Convert(level), message.ToString());
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