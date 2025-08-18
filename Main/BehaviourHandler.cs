using PeakCheat.Types;
using PeakCheat.Utilities;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PeakCheat.Main
{
    internal class BehaviourHandler : MonoBehaviour
    {
        private static List<CheatBehaviour> _behaviours = new List<CheatBehaviour>();
        public static CheatBehaviour[] Behaviours => _behaviours.ToArray();
        public static Dictionary<Type, CheatBehaviour> BehaviourDict => Behaviours.ToDictionary(C => C.GetType());
        public static bool TryGetBehaviour<T>(out T behaviour) where T : CheatBehaviour
        {
            if (BehaviourDict.TryGetValue(typeof(T), out var _behaviour))
            {
                behaviour = (T)_behaviour;
                return true;
            }

            behaviour = default!;
            return false;
        }
        private bool CreateBehaviour(Type type, out CheatBehaviour? behaviour)
        {
            if (BehaviourDict.TryGetValue(type, out behaviour)) return true;
            if (!type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                Debug.Log($"Initialized CheatBehaviour for: {type.Name}");
                behaviour = (CheatBehaviour)Activator.CreateInstance(type);
                return true;
            }
            var obj = new GameObject(type.Name, type);
            obj.transform.parent = transform;
            if (!obj.TryGetComponent(type, out var c)) c = obj.AddComponent(type);
            Debug.Log($"Created child object for behaviour \"{type.Name}\"");
            behaviour = (CheatBehaviour)c;
            return true;
        }
        public static T[] TryGetBehaviours<T>() where T : CheatBehaviour => BehaviourDict.Where(P => P.Key.GetInterface(typeof(T).Name) != null)?.Select(P => (T)P.Value).ToArray()?? Array.Empty<T>();
        public void Awake()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                if (type.GetInterface("CheatBehaviour") != null &&
                    !type.IsAbstract &&
                    !type.IsInterface && 
                    CreateBehaviour(type, out var result) && 
                    result is CheatBehaviour behaviour)
                    _behaviours.AddIfNew(behaviour);
            Behaviours.Where(C => !C.DelayStart()).ForEach(C => C.Start());
            Behaviours.Where(C => C.DelayStart()).ForEach(C => C.Start());
        }
        public void Start() => Behaviours.ForEach(C => C.OnLoad());
        public void Update() => Behaviours.ForEach(C => C.Update());
    }
}