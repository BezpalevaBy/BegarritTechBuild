using System.Collections.Generic;
using UnityEngine;

public enum RoomType { City, MainCorridor, SideRoom, TransitionToNextFloor }

[System.Serializable]
public class DungeonRoomNode
{
    public string id;
    public RoomType type;
    public Vector2 position; // Координаты на виртуальной сетке/карте
    public List<DungeonRoomNode> connectedNodes = new List<DungeonRoomNode>();
    
    // Ссылка на созданный объект в игре
    [HideInInspector] public GameObject spawnedInstance;

    public DungeonRoomNode(string id, RoomType type, Vector2 position)
    {
        this.id = id;
        this.type = type;
        this.position = position;
    }
}