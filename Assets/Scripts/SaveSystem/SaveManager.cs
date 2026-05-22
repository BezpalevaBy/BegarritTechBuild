using UnityEngine;
using Player;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public PlayerStats playerStatsHealth;
    public GameObject playerObject;
    
    [Header("Current Progress")]
    public int currentDungeonLevel;
    public bool isStoryMode;
    public List<string> activeFlags = new List<string>();

    private const string SAVE_KEY = "AdelarGameSave";

    public void SaveGame()
    {
        SaveData data = new SaveData();

        data.health = playerStatsHealth.health;
        data.playerPosition = playerObject.transform.position;
        
        data.lastDungeonLevel = currentDungeonLevel;
        data.isStorySave = isStoryMode;
        data.completedFlags = new List<string>(activeFlags);

        string json = data.ToJson();
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        
        Debug.Log("Игра сохранена: " + json);
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.LogWarning("Нет данных для загрузки!");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = SaveData.FromJson(json);

        playerStatsHealth.health = data.health;
        playerObject.transform.position = data.playerPosition;
        
        currentDungeonLevel = data.lastDungeonLevel;
        isStoryMode = data.isStorySave;
        activeFlags = data.completedFlags;

        playerStatsHealth.Invoke("UpdateVignetteAlpha", 0.1f);
    }

    public void AddFlag(string flagName)
    {
        if (!activeFlags.Contains(flagName))
            activeFlags.Add(flagName);
    }
}