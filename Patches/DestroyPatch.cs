using HarmonyLib;
using PeakCheat.Utilities;
using Photon.Pun;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zorro.Core;

namespace PeakCheat.Patches
{
    [HarmonyPatch(typeof(PhotonNetwork), "RemoveInstantiatedGO")]
    internal class DestroyPatch
    {
        public static void Postfix(GameObject go, bool localOnly)
        {
            if (go.TryGetComponent<Character>(out var c) && c.IsLocal) CreateCharacter();
        }
        private static async void CreateCharacter()
        {
            await Task.Delay(1000);

            if (!PhotonNetwork.InRoom) return;
            if (UnityEngine.Object.FindFirstObjectByType<CharacterSpawner>() is CharacterSpawner spawner)
            {
                spawner.hasSpawnedPlayer = false;
                spawner.photonView.RPC("SpawnPlayerRPC", PhotonNetwork.LocalPlayer, Singleton<ReconnectHandler>.Instance?.GetReconnectData(PhotonNetwork.LocalPlayer) ?? new ReconnectData()
                {
                    dead = false,
                    isValid = true,
                    deathTimer = 0f,
                    fullyPassedOut = false,
                    position = PlayerUtil.GetRespawnPosition(),
                    inventorySyncData = new InventorySyncData(),
                    mapSegment = Singleton<MapHandler>.Instance.GetCurrentSegment(),
                    currentStatuses = new bool[Enum.GetValues(typeof(CharacterAfflictions.STATUSTYPE)).Length].Select(B => 0f).ToArray()
                }, false);

                LogUtil.Log($"Someone tried to destroy your player!");
                return;
            }

            LogUtil.Log("Cant locate spawner object!");
            PhotonNetwork.LeaveRoom(true);
        }
    }
}