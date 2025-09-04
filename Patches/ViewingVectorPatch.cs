using HarmonyLib;
using System;
using UnityEngine;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(Quaternion), nameof(Quaternion.LookRotation), new Type[]
    {
        typeof(Vector3),
        typeof(Vector3)
    })]
    internal class ViewingVectorPatch
    {
        static bool Prefix(ref Vector3 forward, ref Vector3 upwards)
        {
            if (forward == Vector3.zero) forward = Vector3.forward;
            if (upwards == Vector3.zero) upwards = Vector3.up;

            return true;
        }
    }
}