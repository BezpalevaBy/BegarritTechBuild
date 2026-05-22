using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float health;
    public Vector3 playerPosition;
    
    public int lastDungeonLevel;
    public bool isStorySave;
    
    public List<string> completedFlags = new List<string>();

    public string ToJson() => JsonUtility.ToJson(this);
    public static SaveData FromJson(string json) => JsonUtility.FromJson<SaveData>(json);
}