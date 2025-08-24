using ExitGames.Client.Photon;
using HarmonyLib;
using PeakCheat.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(PhotonPeer), "SendOperation", new Type[]
    {
        typeof(byte), typeof(ParameterDictionary), typeof(SendOptions)
    })]
    public class OperationPatch
    {
        private struct EventPatcher
        {
            public EventPatcher(string Id, byte code, Func<ParameterDictionary, ParameterDictionary> method)
            {
                ID = Id;
                Code = code;
                Method = method;
            }
            public string ID;
            public byte Code;
            public Func<ParameterDictionary, ParameterDictionary> Method;
        }
        private static readonly List<EventPatcher> eventPatches = new List<EventPatcher>();
        public const string TrueString = "Return: True";
        public const string FalseString = "Return: False";
        public static void AddEventPatch(byte eventCode, Func<ParameterDictionary, ParameterDictionary> method, string ID) => eventPatches.Add(new EventPatcher(ID, eventCode, method));
        public static bool ContainsEventPatch(string ID) => eventPatches.Any(T => T.ID == ID);
        public static void RemoveEventPatch(string ID)
        {
            if (eventPatches.Any(T => T.ID == ID, out var t))
                eventPatches.Remove(t);
        }
        public static bool Prefix(byte operationCode, ref ParameterDictionary operationParameters, SendOptions sendOptions)
        {
            if (operationCode == 253)
                if (operationParameters.TryGetValue(244, out byte eventCode))
                    foreach (var patch in eventPatches)
                        if (patch.Code == eventCode)
                        {
                            var paramDict = patch.Method?.Invoke(operationParameters) ?? operationParameters;
                            if (paramDict != null)
                            {
                                if (paramDict.Any(P => P.Value is string str && str == FalseString)) return false;
                                if (paramDict.Any(P => P.Value is string str && str == TrueString)) return true;
                                operationParameters = paramDict;
                            }
                        }
            return true;
        }
    }
}