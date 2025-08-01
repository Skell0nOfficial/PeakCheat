﻿using BepInEx;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace PeakCheat.Main
{
    [BepInPlugin("org.skellon.peak.cheat", "PeakCheat", "0.1")]
    public class Plugin: BaseUnityPlugin
    {
        public static Plugin? Instance { get; set; }
        private static GameObject? _cheatObject;
        public Harmony? Patcher;
        public void Awake()
        {

            (Patcher = new Harmony(Info.Metadata.GUID)).PatchAll();
            _cheatObject = new GameObject(Info.Metadata.Name, typeof(Main));
            _cheatObject.hideFlags = HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(_cheatObject);
        }
    }
}