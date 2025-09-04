using PeakCheat.Types;
using PeakCheat.Utilities;
using Photon.Pun;
using UnityEngine;

namespace PeakCheat.Cheats.Abusive
{
    internal class BreakNames: Cheat
    {
        public override string Name => "Break Names";
        public override string Description => "Makes all nametags whatever you have copied";
        public override void Enable()
        {
            if (!PhotonNetwork.InRoom) return;

            string str = string.Empty;
            int num = 5;

            for (int i = 0; i < short.MaxValue - 2; i++)
            {
                if (num == 0)
                {
                    str += '\n';
                    num = 10;
                    continue;
                }

                str += string.IsNullOrEmpty(GUIUtility.systemCopyBuffer)? "PeakCheat": GUIUtility.systemCopyBuffer;
                num--;
            }

            foreach (var player in PhotonNetwork.PlayerList) Exploits.ForceSetName(player.ActorNumber, str[..(short.MaxValue / 3)]);
            LogUtil.Log("Completed Serialization");
            Enabled = false;
        }
    }
}
