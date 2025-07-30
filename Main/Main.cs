using PeakCheat.Classes;
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
        public void Awake()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                if (type.IsSubclassOf(typeof(CheatBehaviour)) && !type.IsAbstract)
                    _behaviours[type] = (CheatBehaviour)Activator.CreateInstance(type);
            _behaviours.Values.ForEach(C => C.Start());
        }
        public void Update() => _behaviours.Values.ForEach(C => C.Update());
        public void OnGUI() => _behaviours.Values.ForEach(C => C.RenderUI());
    }
}