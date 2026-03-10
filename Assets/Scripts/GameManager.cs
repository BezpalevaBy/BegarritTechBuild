using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class GameManager
    {
        public static void ChangeScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        public static void QuitGame()
        {
            StopGameManager.ResumeGame();
            
            if (SceneManager.GetActiveScene().name != "GameMenu")
            {
                //SceneManager.UnloadSceneAsync();
                SceneManager.LoadScene("GameMenu");
                return;
            }

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Просто останавливаем режим Play
#endif

            // 2. Если это финальная сборка игры (Build)
            Application.Quit(); // Эта команда закроет приложение
        }
    }
}