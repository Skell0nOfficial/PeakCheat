using PeakCheat.Classes;
using PeakCheat.Utilities;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace PeakCheat.Cheats.Visuals
{
    internal class ScreenShot: Cheat
    {
        public override string Name => "Screen Shotter";
        public override string Description => "Takes an Image of every player then opens it";
        public override void Enable()
        {
            CaptureAll();
            Enabled = false;
        }
        private static async void CaptureAll()
        {
            foreach (var player in PlayerUtil.AllPlayers())
                await CaptureScreen(player);
        }
        private static async Task CaptureScreen(CheatPlayer player)
        {
            var head = player.HeadTransform;
            if (head == null) return;
            var obj = new GameObject($"tempCam:{player.Name}", typeof(Camera)).transform;
            obj.position = head.position + (head.forward * 3f);
            obj.LookAt(head);
            string path = $"{player.Name} Image.png";
            await File.WriteAllBytesAsync(path, (await UnityUtil.CaptureImage(obj.GetComponent<Camera>()) ?? Texture2D.grayTexture).EncodeToPNG());
            GameObject.DestroyImmediate(obj.gameObject);
            Process.Start(Path.GetFullPath(path));
            GeneralUtil.DelayInvoke(() => File.Delete(path), 1f);
        }
    }
}