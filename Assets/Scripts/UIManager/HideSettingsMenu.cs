using UnityEngine;
using UnityEngine.UI; // Обязательно для работы с компонентом Button

[RequireComponent(typeof(Button))] // Скрипт сам добавит компонент Button, если его нет
public class HideSettingsMenu : MonoBehaviour
{
    [Header("Настройки Иерархии")]
    [Tooltip("Перетащи сюда корневой объект меню Настроек (тот, который нужно скрыть)")]
    public GameObject settingsMenuRoot;

    // Внутренняя ссылка на кнопку на этом объекте
    private Button closeButton;

    public void Awake()
    {
        // 1. Получаем ссылку на компонент Button на этом же объекте
        closeButton = GetComponent<Button>();

        // 2. Проверяем, назначена ли ссылка на меню в Инспекторе
        if (settingsMenuRoot == null)
        {
            Debug.LogError($"<color=red>[Ошибка]</color> На объекте {gameObject.name} в скрипте {this.GetType().Name} не назначена ссылка на 'Settings Menu Root'!");
            // Выключаем скрипт, чтобы избежать дальнейших ошибок
            enabled = false;
            return;
        }

        // 3. Подписываемся на событие нажатия кнопки программно
        // Это надежнее, чем настраивать OnClick() в Инспекторе вручную
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    /// <summary>
    /// Метод, который вызывается при клике на кнопку.
    /// </summary>
    public void OnCloseButtonClicked()
    {
        // Логируем для отладки (потом можно отключить)
        Debug.Log($"<color=green>[Меню]</color> Кнопка закрытия нажата. Скрываю меню: {settingsMenuRoot.name}");

        // Собственно, делаем объект и всех его детей невидимыми и неактивными

        ShowSettingsMenu.IsAlrOpenedWindow = false;
        
        settingsMenuRoot.SetActive(false);
    }

    public void OnDestroy()
    {
        // Хорошая практика: отписываться от событий при уничтожении объекта
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }
    }
}