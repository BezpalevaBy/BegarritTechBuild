using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Настройки Сида")]
    [SerializeField] private string stringSeed = "MyAwesomeDungeon123";
    [SerializeField] private bool useRandomSeed = false;

    [Header("Настройки этажей")]
    [SerializeField] private int totalFloors = 3;
    [SerializeField] private int mainCorridorLength = 4; // Сколько сегментов в главном коридоре
    [SerializeField] private int maxSideRoomsPerFloor = 3;

    [Header("Шаг сетки (Размеры комнат в метрах)")]
    [SerializeField] private Vector2 roomSpacing = new Vector2(20f, 15f);

    [Header("Префабы комнат данжа")]
    [SerializeField] private GameObject entrancePrefab; // Вместо города теперь комната-вход на этаж
    [SerializeField] private GameObject[] mainCorridorPrefabs;
    [SerializeField] private GameObject[] sideRoomPrefabs;
    [SerializeField] private GameObject[] transitionPrefabs; // Комната-выход/спуск на след. этаж

    private List<DungeonRoomNode> dungeonGraph = new List<DungeonRoomNode>();

    public List<DungeonRoomNode> GetGraph() => dungeonGraph;

    [ContextMenu("Сгенерировать Данж")]
    public void GenerateDungeon()
    {
        ClearDungeon();
        InitSeed();
        BuildGraphStructure();
        SpawnRoomPrefabs();
    }

    private void Awake()
    {
        if (Application.isPlaying)
        {
            GenerateDungeon();
        }
    }
    
    private void InitSeed()
    {
        if (useRandomSeed)
        {
            stringSeed = Random.Range(0, 1000000).ToString();
        }
        int seed = stringSeed.GetHashCode();
        Random.InitState(seed);
        Debug.Log($"Данж генерируется под сидом: {stringSeed} (Hash: {seed})");
    }

    private void BuildGraphStructure()
    {
        // 1. Создаем единственный Вход (Entrance) для ВСЕГО данжа в самом начале
        // Он создается один раз перед циклом этажей
        float firstFloorY = 0f;
        Vector2 globalEntrancePos = new Vector2(-1.5f, firstFloorY);
        DungeonRoomNode entranceNode = new DungeonRoomNode("F1_Entrance", RoomType.City, globalEntrancePos);
        dungeonGraph.Add(entranceNode);

        // Узел, к которому будет привязываться начало коридора текущего этажа.
        // Для первого этажа — это наша единственная комната-вход.
        DungeonRoomNode previousFloorConnection = entranceNode;

        // Генерируем этажи
        for (int floor = 1; floor <= totalFloors; floor++)
        {
            List<DungeonRoomNode> currentFloorCorridors = new List<DungeonRoomNode>();
            float floorY = -(floor - 1) * 3; // Каждый этаж смещается ниже по виртуальной сетке Y

            // 2. Строим Главный Коридор (Магистраль) текущего этажа
            for (int i = 0; i < mainCorridorLength; i++)
            {
                Vector2 corridorPos = new Vector2(i * 1.5f, floorY);
                DungeonRoomNode corridorNode = new DungeonRoomNode($"F{floor}_Corridor_{i}", RoomType.MainCorridor, corridorPos);
                
                if (currentFloorCorridors.Count > 0)
                {
                    corridorNode.connectedNodes.Add(currentFloorCorridors[currentFloorCorridors.Count - 1]);
                    currentFloorCorridors[currentFloorCorridors.Count - 1].connectedNodes.Add(corridorNode);
                }

                currentFloorCorridors.Add(corridorNode);
                dungeonGraph.Add(corridorNode);
            }

            // Связываем прошлую точку подключения с началом нового коридора.
            // На 1-м этаже: свяжет Entrance -> Corridor_0.
            // На 2-м и далее этажах: свяжет Exit предыдущего этажа -> Corridor_0 текущего этажа напрямую!
            previousFloorConnection.connectedNodes.Add(currentFloorCorridors[0]);
            currentFloorCorridors[0].connectedNodes.Add(previousFloorConnection);

            // 3. Создаем случайные боковые комнаты (вешаем под коридорами)
            int sideRoomsSpawned = 0;
            
            List<DungeonRoomNode> shuffledCorridors = new List<DungeonRoomNode>(currentFloorCorridors);
            for (int i = shuffledCorridors.Count - 1; i > 0; i--)
            {
                int k = Random.Range(0, i + 1);
                var value = shuffledCorridors[k];
                shuffledCorridors[k] = shuffledCorridors[i];
                shuffledCorridors[i] = value;
            }
            
            foreach (var corridor in shuffledCorridors)
            {
                if (sideRoomsSpawned >= maxSideRoomsPerFloor) break;
                
                if (Random.value > 0.4f)
                {
                    Vector2 sideRoomPos = corridor.position + new Vector2(0, -0.7f); // Смещаем вниз
                    DungeonRoomNode sideRoom = new DungeonRoomNode($"F{floor}_SideRoom_{sideRoomsSpawned}", RoomType.SideRoom, sideRoomPos);
                    
                    corridor.connectedNodes.Add(sideRoom);
                    sideRoom.connectedNodes.Add(corridor);

                    dungeonGraph.Add(sideRoom);
                    sideRoomsSpawned++;
                }
            }

            // 4. Создаем Выход / Переход на следующий этаж
            // Смещаем вправо и немного вниз, формируя спуск, как на твоем рисунке
            Vector2 transitionPos = currentFloorCorridors[currentFloorCorridors.Count - 1].position + new Vector2(1.5f, -1.0f);
            DungeonRoomNode transitionNode = new DungeonRoomNode($"F{floor}_Exit", RoomType.TransitionToNextFloor, transitionPos);
            
            currentFloorCorridors[currentFloorCorridors.Count - 1].connectedNodes.Add(transitionNode);
            transitionNode.connectedNodes.Add(currentFloorCorridors[currentFloorCorridors.Count - 1]);
            
            dungeonGraph.Add(transitionNode);

            // Запоминаем этот выход как точку подключения для следующего этажа
            previousFloorConnection = transitionNode;
        }
    }

    private void SpawnRoomPrefabs()
    {
        foreach (var node in dungeonGraph)
        {
            // КОРРЕКЦИЯ ПОЗИЦИИ: Прибавляем transform.position родительского объекта,
            // чтобы спавнить комнаты относительно точки расположения самого генератора на сцене.
            Vector3 worldPos = transform.position + new Vector3(node.position.x * roomSpacing.x, node.position.y * roomSpacing.y, 0);
            GameObject prefabToSpawn = null;

            switch (node.type)
            {
                case RoomType.City: prefabToSpawn = entrancePrefab; break; // Здесь спавнится комната-вход
                case RoomType.MainCorridor: prefabToSpawn = GetRandomPrefab(mainCorridorPrefabs); break;
                case RoomType.SideRoom: prefabToSpawn = GetRandomPrefab(sideRoomPrefabs); break;
                case RoomType.TransitionToNextFloor: prefabToSpawn = GetRandomPrefab(transitionPrefabs); break;
            }

            if (prefabToSpawn != null)
            {
                GameObject instance = Instantiate(prefabToSpawn, worldPos, Quaternion.identity, transform);
                instance.name = node.id;
                node.spawnedInstance = instance;
            }
        }
    }

    private GameObject GetRandomPrefab(GameObject[] array)
    {
        if (array == null || array.Length == 0) return null;
        return array[Random.Range(0, array.Length)];
    }

    public void ClearDungeon()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        dungeonGraph.Clear();
    }
}