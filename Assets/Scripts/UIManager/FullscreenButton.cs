using UnityEngine;
using UnityEngine.UI; // Обязательно для работы с Toggle

[RequireComponent(typeof(Toggle))] // Скрипт сам добавит Toggle, если его нет
public class SettingsResolution : MonoBehaviour
{
    // Ключ для сохранения настройки в памяти
    private const string FullscreenPrefKey = "IsFullscreen";
    
    // Внутренняя ссылка на компонент Toggle на этом объекте
    private Toggle fullscreenToggle;

    private void Awake()
    {
        fullscreenToggle = GetComponent<Toggle>();

        // 1. Загружаем сохраненную настройку (по умолчанию true - включено)
        bool isFullscreenSaved = PlayerPrefs.GetInt(FullscreenPrefKey, 1) == 1;

        // 2. Устанавливаем визуальное состояние галочки БЕЗ вызова события изменения
        // (чтобы не применять настройку дважды при старте)
        fullscreenToggle.SetIsOnWithoutNotify(isFullscreenSaved);

        // 3. Сразу применяем сохраненную настройку разрешения
        ApplyFullscreen(isFullscreenSaved);

        // 4. Подписываемся на изменение состояния галочки программно
        fullscreenToggle.onValueChanged.AddListener(OnToggleClicked);
    }

    /// <summary>
    /// Метод, вызываемый при клике на галочку.
    /// </summary>
    private void OnToggleClicked(bool isOn)
    {
        // Логируем для отладки
        Debug.Log($"<color=cyan>[Видео]</color> Полноэкранный режим: {(isOn ? "ВКЛ" : "ВЫКЛ")}");

        // Применяем настройку
        ApplyFullscreen(isOn);

        // Сохраняем настройку в память (1 - true, 0 - false)
        PlayerPrefs.SetInt(FullscreenPrefKey, isOn ? 1 : 0);
        PlayerPrefs.Save(); // Принудительно сохраняем на диск
    }

    /// <summary>
    /// Собственно, метод применения полноэкранного режима в Unity.
    /// </summary>
    private void ApplyFullscreen(bool state)
    {
        // Screen.fullScreenMode дает больше контроля, но Screen.fullScreen проще
        Screen.fullScreen = state;
    }

    private void OnDestroy()
    {
        // Хорошая практика: отписываться от событий
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(OnToggleClicked);
        }
    }
}