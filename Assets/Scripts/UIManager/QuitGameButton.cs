using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Обязательно для работы с компонентом Button

[RequireComponent(typeof(Button))] // Скрипт сам добавит компонент Button, если его нет
public class QuitGameButton : MonoBehaviour
{
    // Внутренняя ссылка на кнопку на этом объекте
    private Button quitButton;

    private void Awake()
    {
        // 1. Получаем ссылку на компонент Button на этом же объекте
        quitButton = GetComponent<Button>();

        // 2. Подписываемся на событие нажатия кнопки программно
        // Это надежнее, чем настраивать OnClick() в Инспекторе вручную
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    /// <summary>
    /// Метод, который вызывается при клике на кнопку.
    /// </summary>
    public void OnQuitButtonClicked()
    {
        // Логируем для отладки
        Debug.Log("<color=red>[Игра]</color> Кнопка выхода нажата. Выхожу...");

        // Собственно, логика выхода
        Quit();
    }

    /// <summary>
    /// Метод выхода из игры. Включает логику для Редактора и Сборки.
    /// </summary>
    public void Quit()
    {
        GameManager.QuitGame();
    }

    public void OnDestroy()
    {
        // Хорошая практика: отписываться от событий при уничтожении объекта
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }
    }
}