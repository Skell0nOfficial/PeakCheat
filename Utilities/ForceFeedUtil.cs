using PeakCheat.Types;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace PeakCheat.Utilities
{
    public class ForceFeedUtil : CheatBehaviour
    {
        private static Dictionary<string, int> _itemViews = new Dictionary<string, int>();
        bool CheatBehaviour.DelayStart() => true;
        void CheatBehaviour.Start() => PhotonCallbacks.LeftRoom += () => _itemViews.Clear();
        public static void ForceFeed(CheatPlayer player, string item)
        {
            if (!_itemViews.TryGetValue(item, out int viewID)) viewID = GetItemView(item);
            player.PlayerRPC("GetFedItemRPC", player, viewID);
            _itemViews.Remove(item);
        }
        private static int GetItemView(string item)
        {
            if (!_itemViews.TryGetValue(item, out var viewID))
            {
                var obj = PhotonNetwork.InstantiateItem(item.Replace("(Clone)", ""), Vector3.zero, Quaternion.identity);
                string name = $"Item \"{obj.name.Replace("(Clone)", "").Replace('_', ' ')}\"";
                if (obj == null)
                {
                    LogUtil.Log(false, $"Failed to create PhotonView for {name}");
                    return -1;
                }
                if (!obj.TryGetComponent<PhotonView>(out var view))
                {
                    LogUtil.Log(false, $"Failed to get PhotonView component for {name}");
                    return -1;
                }
                viewID = view.ViewID;
                _itemViews[item] = viewID;
                LogUtil.Log($"Created PhotonView for {name} ({viewID})");
            }
            return viewID;
        }
    }
}