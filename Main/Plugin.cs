using BepInEx;
using HarmonyLib;
using PeakCheat.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.UI.Modal;

namespace PeakCheat.Main
{
    [BepInPlugin("org.skellon.peak.cheat", "PeakCheat", "1.0")]
    public class Plugin: BaseUnityPlugin
    {
        public static Plugin? Instance { get; private set; }
        private static GameObject? _cheatObject;
        public Harmony? Patcher;
        private bool blocked;
        public void Awake()
        {
            if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Vulkan)
            {
                blocked = true;
                Application.wantsToQuit += () => !Modal.IsOpen;
                return;
            }

            Instance = this;
            (Patcher = new Harmony(Info.Metadata.GUID)).PatchAll();
            _cheatObject = new GameObject(Info.Metadata.Name, typeof(BehaviourHandler));
            _cheatObject.hideFlags = HideFlags.HideAndDontSave;

            DontDestroyOnLoad(_cheatObject);
        }
        public void Start()
        {
            if (blocked)
            {
                Modal.OpenModal(new DefaultHeaderModalOption("PeakCheat".WithColor((Color.white * .4f).WithAlpha(1f)), "<size=25>Sorry, but PeakCheat is only available for Vulkan!</size>\nDX12/DX11 is not supported!"), new ModalButtonsOption(new ModalButtonsOption.Option[]
                {
                    new ModalButtonsOption.Option("Okay", () => Application.Quit())
                }));

                GameObject.Destroy(gameObject);
                SceneManager.GetActiveScene().GetRootGameObjects().Execute(GameObject.Destroy);
            }
        }
    }
}