using UnityEngine;

[ExecuteInEditMode] // Чтобы работало без запуска игры
public class DungeonVisualizer : MonoBehaviour
{
    private DungeonGenerator generator;

    void OnDrawGizmos()
    {
        if (generator == null) generator = GetComponent<DungeonGenerator>();
        if (generator == null) return;

        var graph = generator.GetGraph();
        if (graph == null || graph.Count == 0) return;

        // Настройки размеров визуализации в Gizmos (чуть меньше реальных комнат для наглядности)
        float spacingX = 20f;
        float spacingY = 15f;

        foreach (var node in graph)
        {
            Vector3 nodeWorldPos = new Vector3(node.position.x * spacingX, node.position.y * spacingY, 0);

            // 1. Выбираем цвет сферы в зависимости от типа комнаты
            switch (node.type)
            {
                case RoomType.City: Gizmos.color = Color.white; break; // Город
                case RoomType.MainCorridor: Gizmos.color = Color.cyan; break; // Главный коридор
                case RoomType.SideRoom: Gizmos.color = Color.green; break; // Боковые тупики
                case RoomType.TransitionToNextFloor: Gizmos.color = Color.red; break; // Переход вниз
            }

            // Рисуем комнату в виде куба или сферы
            Gizmos.DrawWireCube(nodeWorldPos, new Vector3(8f, 5f, 0));
            Gizmos.DrawSphere(nodeWorldPos, 0.8f);

            // 2. Рисуем связи (проходы между комнатами)
            Gizmos.color = Color.yellow;
            foreach (var connectedNode in node.connectedNodes)
            {
                Vector3 connectedWorldPos = new Vector3(connectedNode.position.x * spacingX, connectedNode.position.y * spacingY, 0);
                // Чтобы линии не дублировались, рисуем только в одну сторону
                if (node.position.x < connectedNode.position.x || node.position.y > connectedNode.position.y)
                {
                    Gizmos.DrawLine(nodeWorldPos, connectedWorldPos);
                }
            }
        }
    }
}