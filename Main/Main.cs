﻿using PeakCheat.Classes;
using pworld.Scripts.Extensions;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PeakCheat.Main
{
    internal class Main : MonoBehaviour
    {
        private static Dictionary<Type, CheatBehaviour> _behaviours = new Dictionary<Type, CheatBehaviour>();
        public static bool TryGetBehaviour<T>(out T behaviour) where T : CheatBehaviour
        {
            if (_behaviours.TryGetValue(typeof(T), out var _behaviour))
            {
                behaviour = (T)_behaviour;
                return true;
            }
            behaviour = default!;
            return false;
        }
        public void Awake()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                if (type.GetInterface("CheatBehaviour") != null && !type.IsAbstract)
                {
                    if (!type.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        _behaviours[type] = (CheatBehaviour)Activator.CreateInstance(type);
                        Debug.Log($"Initialized CheatBehaviour for: {type.Name}");
                        continue;
                    }
                    if (!gameObject.TryGetComponent(type, out _))
                        gameObject.AddComponent(type);
                    Debug.Log($"Created child object for behaviour \"{type.Name}\"");
                }
            _behaviours.Values.ForEach(C => C.Start());
        }
        public void Update() => _behaviours.Values.ForEach(C => C.Update());
        public void OnGUI() => _behaviours.Values.ForEach(C => C.RenderUI());
    }
}