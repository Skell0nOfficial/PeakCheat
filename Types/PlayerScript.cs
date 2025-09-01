using PeakCheat.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PeakCheat.Types
{
    internal class PlayerScript: Attribute
    {
        private static Dictionary<string, MethodInfo>? _cachedMethods = new Dictionary<string, MethodInfo>();
        public static void ExecuteScript(CheatPlayer player, KeyValuePair<string, MethodInfo> pair)
        {
            var name = pair.Key;
            var method = pair.Value;
            object[] objects = new object[0];

            if (method.GetParameters().Length == 1)
            {
                var param = method.GetParameters()[0];
                if (param == null) return;
                if (param.ParameterType == typeof(Vector3)) objects = new object[] { player.Center };
                else if (param.ParameterType == typeof(CheatPlayer)) objects = ((object)player).SingleArray();
            }

            try
            {
                var result = method.Invoke(null, objects);

                if (method.ReturnType == typeof(void))
                {
                    LogUtil.Log($"Invoked {name}, no return value");
                    return;
                }

                if (result is bool value)
                {
                    LogUtil.Log($"Invoked {name}, result: {value.ToString().ToLower()}");
                    return;
                }

                LogUtil.Log($"Invoked {name}, result: {result}");
            }
            catch (Exception error)
            {
                LogUtil.Log(true, $@"
CANT INVOKE {name.ToUpper()}


Message: {error.Message}
Source: {error.Source}");
            }
        }
        public static Dictionary<string, MethodInfo> GetMethods()
        {
            if (_cachedMethods != null && _cachedMethods.Count != 0) return _cachedMethods;

            var methods = new Dictionary<string, MethodInfo>();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type != null && type.GetCustomAttribute(typeof(ScriptBase)) != null)
                {
                    foreach (var method in type.GetMethods())
                    {
                        if (method == null ||
                            !method.IsStatic ||
                            method.DeclaringType != type ||
                            method.Name.Contains("get_") ||
                            method.GetParameters().Length != 1 ||
                            method.GetParameters()[0].ParameterType != typeof(CheatPlayer)) continue;

                        if (method.GetCustomAttribute<PlayerScript>() is PlayerScript playerScript)
                        {
                            methods[playerScript.Name] = method;
                            continue;
                        }
                    }
                }
            }

            var result = new Dictionary<string, MethodInfo>();

            foreach (var key in methods.Keys.ToArray().OrderBy(S => S.Length)) result.Add(key, methods[key]);
            methods = result;

            _cachedMethods = methods;
            return methods;
        }
        public string Name { get; private set; }
        public PlayerScript(string name) => Name = name;
    }
}
