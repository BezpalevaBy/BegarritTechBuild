using UnityEngine;
using TMPro;  // 重要：必须添加这个命名空间
using System.Collections.Generic;
using System.Linq;

public class ResolutionDropdown : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;  // 改为 TMP_Dropdown
    
    [Header("Settings")]
    [SerializeField] private bool showOnlySupportedResolutions = true;
    [SerializeField] private bool saveAndLoadResolution = true;
    [SerializeField] private string playerPrefsKey = "Resolution";
    
    private List<Resolution> resolutions = new List<Resolution>();
    private List<string> resolutionOptions = new List<string>();
    
    void Start()
    {
        // 获取 TMP_Dropdown 组件
        if (resolutionDropdown == null)
        {
            resolutionDropdown = GetComponent<TMP_Dropdown>();
            
            if (resolutionDropdown == null)
            {
                resolutionDropdown = GetComponentInChildren<TMP_Dropdown>();
            }
            
            if (resolutionDropdown == null)
            {
                Debug.LogError("ResolutionDropdown: TMP_Dropdown component not found!");
                Debug.LogWarning("Please add a TMP_Dropdown component to this GameObject.");
                return;
            }
        }
        
        InitializeResolutionDropdown();
    }
    
    void InitializeResolutionDropdown()
    {
        // Получаем все доступные разрешения
        if (showOnlySupportedResolutions)
        {
            resolutions = Screen.resolutions.ToList();
        }
        else
        {
            resolutions = GetPopularResolutions();
        }
    
        // Фильтруем дубликаты и сортируем
        resolutions = resolutions
            .GroupBy(r => new { r.width, r.height, r.refreshRateRatio })
            .Select(g => g.First())
            .OrderByDescending(r => r.width)
            .ThenByDescending(r => r.height)
            .ToList();
    
        // Создаем список опций для выпадающего меню
        resolutionOptions.Clear();
        int currentResolutionIndex = 0;
    
        for (int i = 0; i < resolutions.Count; i++)
        {
            Resolution res = resolutions[i];
            // ИЗМЕНЕНО: убрали @ и Hz
            string optionText = $"{res.width} x {res.height}";
            resolutionOptions.Add(optionText);
        
            // Проверяем текущее разрешение
            if (res.width == Screen.currentResolution.width && 
                res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
    
        // Заполняем выпадающее меню
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);
    
        // Загружаем сохраненное разрешение
        if (saveAndLoadResolution && PlayerPrefs.HasKey(playerPrefsKey))
        {
            int savedResolutionIndex = PlayerPrefs.GetInt(playerPrefsKey);
            if (savedResolutionIndex >= 0 && savedResolutionIndex < resolutions.Count)
            {
                currentResolutionIndex = savedResolutionIndex;
            }
        }
    
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    
        ApplyResolution(currentResolutionIndex);
    }
    
    void OnResolutionChanged(int index)
    {
        ApplyResolution(index);
        
        if (saveAndLoadResolution)
        {
            PlayerPrefs.SetInt(playerPrefsKey, index);
            PlayerPrefs.Save();
        }
    }
    
    void ApplyResolution(int index)
    {
        if (index >= 0 && index < resolutions.Count)
        {
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
            Debug.Log($"Resolution changed to: {resolution.width}x{resolution.height}");
        }
    }
    
    List<Resolution> GetPopularResolutions()
    {
        List<Resolution> popularResolutions = new List<Resolution>();
        
        var commonResolutions = new (int width, int height, int refreshRate)[]
        {
            (3840, 2160, 60), (2560, 1440, 60), (1920, 1080, 60),
            (1920, 1080, 144), (1600, 900, 60), (1366, 768, 60),
            (1280, 720, 60), (1280, 1024, 60), (1024, 768, 60)
        };
        
        foreach (var res in commonResolutions)
        {
            Resolution newResolution = new Resolution
            {
                width = res.width,
                height = res.height,
                refreshRateRatio = new RefreshRate() { numerator = (uint)res.refreshRate, denominator = 1 }
            };
            popularResolutions.Add(newResolution);
        }
        
        return popularResolutions;
    }
}