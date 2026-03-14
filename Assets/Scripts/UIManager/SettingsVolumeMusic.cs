using System;
using UnityEngine;
using UnityEngine.UI; // Обязательно для работы со Slider

[RequireComponent(typeof(Slider))] // Скрипт сам добавит Slider, если его нет
public class SettingsVolumeMusic : MonoBehaviour
{
    // Ключ для сохранения настройки в памяти (другой ключ!)
    private const string MusicVolumePrefKey = "VolumeMusic";

    [Header("Аудио")]
    [Tooltip("Опционально: Перетащи сюда AudioSource, который отвечает за фоновую музыку.")]
    public AudioSource musicAudioSource;

    // Внутренняя ссылка на компонент Slider на этом объекте
    private Slider volumeSlider;

    private void OnRenderObject()
    {
        Awake();
    }

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();

        // Настраиваем слайдер
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.wholeNumbers = false;

        // 1. Загружаем сохраненную громкость музыки (по умолчанию 0.5f - 50%)
        float savedVolume = PlayerPrefs.GetFloat(MusicVolumePrefKey, 0.5f);

        // 2. Устанавливаем визуальное положение ползунка БЕЗ вызова события
        volumeSlider.SetValueWithoutNotify(savedVolume);

        // 3. Сразу применяем сохраненную громкость
        ApplyVolume(savedVolume);

        // 4. Подписываемся на изменение ползунка программно
        volumeSlider.onValueChanged.AddListener(OnSliderMoved);
    }

    /// <summary>
    /// Метод, вызываемый при движении ползунка музыки.
    /// </summary>
    private void OnSliderMoved(float value)
    {
        // Применяем громкость музыки
        ApplyVolume(value);

        // Сохраняем настройку музыки в память
        PlayerPrefs.SetFloat(MusicVolumePrefKey, value);
    }

    /// <summary>
    /// Применяет громкость к Music AudioSource.
    /// </summary>
    private void ApplyVolume(float volume)
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = volume;
        }
        else
        {
            // Debug.LogWarning($"[Музыка] Громкость {Mathf.RoundToInt(volume * 100)}%, но Music AudioSource не назначен!");
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