using UnityEngine;
using UnityEngine.UI; // Обязательно для работы с компонентом Button

public class ShowSettingsMenu : MonoBehaviour
{
    [Header("Настройки Иерархии")]
    [Tooltip("Перетащи сюда корневой объект меню Настроек (тот, который нужно ОТКРЫТЬ)")]
    public GameObject settingsMenuRoot;

    [Header("Кнопка Вызова")]
    [Tooltip("Перетащи сюда кнопку в Иерархии, которая должна открывать настройки (например, кнопку на пергаменте 'SETTINGS')")]
    public Button openButton;
    
    public static bool IsAlrOpenedWindow = false;

    public void Awake()
    {
        // 1. Проверяем, назначены ли ссылки в Инспекторе

        IsAlrOpenedWindow = false;
        if (settingsMenuRoot == null || openButton == null)
        {
            string missingRef = settingsMenuRoot == null ? "'Settings Menu Root'" : "'Open Button'";
            Debug.LogError($"<color=red>[Ошибка]</color> На объекте {gameObject.name} в скрипте {this.GetType().Name} не назначена ссылка на {missingRef}!");
            // Выключаем скрипт, чтобы избежать дальнейших ошибок
            enabled = false;
            return;
        }

        // 2. Убеждаемся, что при старте меню настроек СКРЫТО (хорошая практика)
        // settingsMenuRoot.SetActive(false); // Раскомментируй эту строку, если хочешь, чтобы меню скрывалось при запуске игры.

        // 3. Подписываемся на событие нажатия кнопки программно
        // Это надежнее, чем настраивать OnClick() в Инспекторе вручную.
        // openButton.onClick.AddListener(OnOpenButtonClicked);
    }

    /// <summary>
    /// Метод, который вызывается при клике на кнопку открытия.
    /// </summary>
    public void OnOpenButtonClicked()
    {
        // Логируем для отладки
        Debug.Log($"<color=blue>[Меню]</color> Кнопка открытия нажата. Открываю меню: {settingsMenuRoot.name}");

        // Собственно, делаем объект и всех его детей видимыми и активными

        if (IsAlrOpenedWindow) return;
        
        IsAlrOpenedWindow = true;
        settingsMenuRoot.SetActive(true);
    }

    public void OnDestroy()
    {
        // Хорошая практика: отписываться от событий при уничтожении объекта
        if (openButton != null)
        {
            // openButton.onClick.RemoveListener(OnOpenButtonClicked);
        }
    }
}