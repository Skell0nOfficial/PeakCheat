using PeakCheat.Types;
using PeakCheat.Utilities;
using pworld.Scripts.Extensions;
using UnityEngine;

namespace PeakCheat.Cheats.Abusive
{
    internal class CrashClicker: Cheat
    {
        public override string Name => "Crasher";
        public override string Description => "Crashes whoever you click";
        private static int _index = 0;
        static int GetIndex(Character c) => GeneralUtil.Compute(c.photonView.Owner.NickName);
        public override void Method()
        {
            if (Camera.main.ScreenPointToRay(Input.mousePosition).Raycast(out var hit))
            {
                var hitObj = hit.collider.gameObject;

                if (!hitObj.TryGetComponent<Character>(out var c) && !hitObj.transform.root.TryGetComponent(out c))
                {
                    if (_index != 0 && Character.AllCharacters.Any(C => _index == GetIndex(C), out var lastHit))
                    {
                        var custom = lastHit?.refs?.customization;
                        custom?.SetCustomizationForRef(custom.refs);
                        _index = 0;
                    }
                    return;
                }

                if (c is Character character)
                {
                    if (character.IsLocal) return;

                    _index = GetIndex(character);

                    if (Input.GetMouseButtonDown(0))
                    {
                        character.ToCheatPlayer().Crash();
                        return;
                    }

                    foreach (var renderer in character.refs.customization.refs.PlayerRenderers)
                        renderer.material.SetColor(Shader.PropertyToID("_SkinColor"),
                            Color.Lerp(Color.white,
                            character.refs.customization.PlayerColor,
                            Mathf.PingPong(Time.time * 2f, 1f)));
                }
            }
        }
    }
}