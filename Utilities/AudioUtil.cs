using PeakCheat.Types;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace PeakCheat.Utilities
{
    internal class AudioUtil: CheatBehaviour
    {
        private static AudioClip? _click;
        public static async void Click()
        {
            if (_click is AudioClip clip)
            {
                if (UnityUtil.TempComponent<AudioSource>(Camera.main.gameObject) is AudioSource source)
                {
                    source.volume = .15f * Mathf.Clamp01(GameHandler.Instance.SettingsHandler.GetSetting<SFXVolumeSetting>().Value);
                    source.PlayOneShot(clip);
                }
                return;
            }

            var path = Application.temporaryCachePath + $"/{UnityEngine.Random.Range(10000, 99999)}.mp3";

            await File.WriteAllBytesAsync(path, Files.click);
            FromPath(path, D =>
            {
                if (D is AudioClip clip)
                {
                    _click = D;
                    Click();
                }

                File.Delete(path);
            });
        }
        public static void FromPath(string path, Action<AudioClip?> onReceived)
        {
            var request = UnityWebRequestMultimedia.GetAudioClip($"file://{path}", AudioType.MPEG);

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    onReceived?.Invoke(DownloadHandlerAudioClip.GetContent(request));
                    request.Dispose();
                    return;
                }

                LogUtil.Log(true, $"Failed to load audio: {request.error}");
                onReceived?.Invoke(null);
                request.Dispose();
            };
        }
    }
}