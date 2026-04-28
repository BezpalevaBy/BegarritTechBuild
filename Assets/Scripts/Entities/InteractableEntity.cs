using UnityEngine;
using UnityEngine.Events;

public class InteractableEntity : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public string hintText = "[E] Открыть сундук";
    public float interactionDuration = 2f; // Сколько секунд нужно держать кнопку 'E' (0 = мгновенно)

    [Header("Событие при успешном завершении")]
    public UnityEvent onInteractComplete;

    // Метод, который вызовется, когда игрок успешно заполнит прогресс-бар
    public void TriggerInteraction()
    {
        Debug.Log($"Взаимодействие с {gameObject.name} успешно завершено!");
        
        // Запускаем все действия, которые ты накидаешь в инспекторе в это событие
        onInteractComplete?.Invoke(); 
    }
}