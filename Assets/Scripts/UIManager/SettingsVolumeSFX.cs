using UnityEngine;
using UnityEngine.UI; // Обязательно для работы со Slider

[RequireComponent(typeof(Slider))] // Скрипт сам добавит Slider, если его нет
public class SettingsVolumeSFX : MonoBehaviour
{
    // Ключ для сохранения настройки в памяти
    private const string SfxVolumePrefKey = "VolumeSFX";

    [Header("Аудио")]
    [Tooltip("Опционально: Перетащи сюда AudioSource, который отвечает за звуковые эффекты.")]
    public AudioSource sfxAudioSource;

    // Внутренняя ссылка на компонент Slider на этом объекте
    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();

        // Настраиваем слайдер (хорошая практика)
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.wholeNumbers = false; // Разрешаем дробные числа

        // 1. Загружаем сохраненную громкость (по умолчанию 0.75f - 75%)
        float savedVolume = PlayerPrefs.GetFloat(SfxVolumePrefKey, 0.75f);

        // 2. Устанавливаем визуальное положение ползунка БЕЗ вызова события
        volumeSlider.SetValueWithoutNotify(savedVolume);

        // 3. Сразу применяем сохраненную громкость
        ApplyVolume(savedVolume);

        // 4. Подписываемся на изменение ползунка программно
        volumeSlider.onValueChanged.AddListener(OnSliderMoved);
    }

    /// <summary>
    /// Метод, вызываемый при движении ползунка.
    /// </summary>
    private void OnSliderMoved(float value)
    {
        // Логируем для отладки (округляем для красоты)
        // Debug.Log($"<color=yellow>[SFX]</color> Громкость: {Mathf.RoundToInt(value * 100)}%");

        // Применяем громкость
        ApplyVolume(value);

        // Сохраняем настройку в память
        PlayerPrefs.SetFloat(SfxVolumePrefKey, value);
    }

    /// <summary>
    /// Применяет громкость к AudioSource.
    /// </summary>
    private void ApplyVolume(float volume)
    {
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = volume;
        }
        else
        {
            // Если AudioSource не назначен, громкость меняется глобально (для теста)
            // Но лучше назначить конкретный AudioSource в Инспекторе
            // AudioListener.volume = volume; // Раскомментируй, если нет AudioSource
        }
    }

    private void OnDestroy()
    {
        // Хорошая практика: отписываться от событий
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnSliderMoved);
        }
    }
}