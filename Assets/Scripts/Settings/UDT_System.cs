using UnityEngine;
using UnityEngine.SceneManagement;

namespace UDT.Settings.System
{
    using static UDT.Settings.UDT_Constants;
    
    public class UDT_System : MonoBehaviour
    {

        /// <summary>
        /// Restarts the current scene.
        /// </summary>
        public void RestartScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
            Time.timeScale = TIME_SCALE_DEFAULT;
        }

        /// <summary>
        /// Starts the game/demo scene.
        /// </summary>
        public void StartGameScene()
        {
            SceneManager.LoadSceneAsync("DemoScene").completed += (operation) =>
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("DemoScene"));
            };
        }

        /// <summary>
        /// Exits the game.
        /// </summary>
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}