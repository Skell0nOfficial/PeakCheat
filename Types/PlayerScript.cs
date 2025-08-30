using System;
using System.Collections.Generic;
using System.Reflection;

namespace PeakCheat.Types
{
    internal class PlayerScript: Attribute
    {
        public static Dictionary<string, MethodInfo> GetMethods()
        {
            var methods = new Dictionary<string, MethodInfo>();

            foreach (var file in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in file.GetTypes())
                    if (type != null && type.GetCustomAttribute(typeof(ScriptBase)) != null)
                        foreach (var method in type.GetMethods())
                        {
                            if (method == null) continue;
                            if (method.DeclaringType != type) continue;
                            if (method.Name.Contains("get_")) continue;
                            if (!method.IsStatic) continue;
                            if (method.GetParameters().Length != 1) continue;
                            if (method.GetParameters()[0].ParameterType != typeof(CheatPlayer)) continue;

                            if (method.GetCustomAttribute<PlayerScript>() is PlayerScript playerScript)
                            {
                                methods[playerScript.Name] = method;
                                continue;
                            }
                        }

            return methods;
        }
        public string Name { get; private set; }
        public PlayerScript(string name) => Name = name;
    }
}
