using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PeakCheat.Cheats.Visuals
{
    internal class ESP
    {
        void SearchRPCs()
        {
            var assembly = typeof(Character).Assembly;
            foreach (var type in assembly.GetTypes())
                foreach (var method in type.GetMethods())
                    foreach (var att in method.CustomAttributes)
                        if (att.AttributeType == typeof(Photon.Pun.PunRPC))
                        {
                            Debug.Log($"Got float RPC: {type.Name}.{method.Name}()");
                            break;
                        }
        }
        void method()
        {
            var renderers = new List<Renderer>();
            var customization = Character.localCharacter.refs.customization;
            var refs = customization.refs;

            renderers.Add(refs.mainRenderer);
            renderers.Add(refs.mainRendererShadow);
            renderers.Add(refs.shorts);
            renderers.Add(refs.shortsShadow);
            renderers.Add(refs.skirt);
            renderers.Add(refs.skirtShadow);
            renderers.Add(refs.PlayerRenderers.First(R => R.name.Contains("Main")));

            foreach (var r in renderers)
            {
                if (!(r.gameObject.activeSelf || r.gameObject.activeInHierarchy)) continue;

                bool esp = !false;

                r.material.shader = Shader.Find(esp ? "GUI/Text Shader" : "W/Character");
                r.material.color = esp ? customization.PlayerColor : Color.white;
            }
        }
    }
}