using Newtonsoft.Json;
using System;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public static class SaveUtil
    {
        public static bool Get<T>(string key, out T obj)
        {
            try
            {
                if (PlayerPrefs.HasKey(key))
                {
                    if (JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(key)) is T result)
                    {
                        obj = result;
                        return true;
                    }

                    LogUtil.Warning = $"got object for key: {key}, didnt match type {typeof(T).Name}";
                    obj = default!;
                    return false;
                }

                LogUtil.Warning = $"Cant find deserialized key: {key}";
                obj = default!;
                return false;
            }
            catch (Exception error)
            {
                LogUtil.Error = $"Cant deserialize key \"{key}\" got error: ({error.Message}::{error.Source})";
                obj = default!;
                return false;
            }
        }
        public static bool Save<T>(this T obj, string key)
        {
            if (obj == null)
            {
                LogUtil.Information = "Cant save object, null";
                return false;
            }
            
            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(obj, Formatting.Indented));
            return true;
        }
    }
}