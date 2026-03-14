using UnityEngine;

public class StopGameManager : MonoBehaviour
{
    public static bool IsPaused = false;

    // Метод DoAction теперь сам решает, что делать, на основе переменной isPaused
    public static void DoAction()
    {
        if (IsPaused) 
            ResumeGame();
        else 
            PauseGame();
    }
    
    public static void PauseGame()
    {
        
        Time.timeScale = 0f;          // Останавливаем время
        IsPaused = true;
    }
    
    public static void ResumeGame()
    {
        Time.timeScale = 1f;           // Возвращаем время
        IsPaused = false;
    }
}