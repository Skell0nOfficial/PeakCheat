using PeakCheat.Types;
using Steamworks;
using UnityEngine.SceneManagement;

namespace PeakCheat.Utilities
{
    internal class CheatUtil: CheatBehaviour
    {
        private static SceneType _currentScene = SceneType.Unknown;
        public static SceneType CurrentScene
        {
            get => _currentScene;
            private set
            {
                _currentScene = value;
                LogUtil.Log($"Switched to scene [{value}]");
            }
        }
        void CheatBehaviour.Start() => SceneManager.sceneLoaded += (_, __) => SceneChanged(_);
        private void SceneChanged(Scene scene)
        {
            var name = scene.name.Trim().ToLower();
            
            if (name.Contains("title"))
            {
                CurrentScene = SceneType.Menu;
                return;
            }

            if (name.Contains("airport"))
            {
                CurrentScene = SceneType.Airport;
                return;
            }

            if (name.Contains("level_") || name.Contains("island"))
            {
                CurrentScene = SceneType.Level;
                return;
            }

            CurrentScene = SceneType.Unknown;
        }
    }
}