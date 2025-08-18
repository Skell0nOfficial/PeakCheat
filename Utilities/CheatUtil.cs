using PeakCheat.Types;
using UnityEngine.SceneManagement;

namespace PeakCheat.Utilities
{
    internal class CheatUtil: CheatBehaviour
    {
        public static SceneType CurrentScene
        {
            get;
            private set;
        }
        void CheatBehaviour.Start()
        {
            SceneManager.sceneLoaded += (_, __) => SceneChanged(_);
        }
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
            if (name.Contains("level_"))
            {
                CurrentScene = SceneType.Level;
                return;
            }
            CurrentScene = SceneType.Unknown;
        }
    }
}