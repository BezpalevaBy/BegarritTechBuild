using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [SerializeField] private GameObject hintObject; // Объект с текстом подсказки
    [SerializeField] private Slider progressSlider; // Слайдер прогресс-бара

    private InteractableEntity currentInteractable; // Текущий объект рядом
    private float currentHoldTime = 0f; // Сколько уже удерживается кнопка
    private bool isInteracting = false; // Идет ли процесс удержания?

    void Update()
    {
        // Если рядом нет объекта для взаимодействия — ничего не делаем
        if (currentInteractable == null) return;

        // Игрок НАЖАЛ и ДЕРЖИТ клавишу Е
        if (Input.GetKey(KeyCode.E))
        {
            // Если у объекта время удержания 0, то взаимодействуем мгновенно при первом нажатии
            if (currentInteractable.interactionDuration <= 0)
            {
                CompleteInteraction();
                return;
            }

            StartInteracting();
        }
        // Игрок ОТПУСТИЛ клавишу Е до завершения прогресса
        else if (Input.GetKeyUp(KeyCode.E))
        {
            ResetInteraction();
        }

        // Если процесс удержания активен, заполняем прогресс-лайн
        if (isInteracting)
        {
            currentHoldTime += Time.deltaTime;
            progressSlider.value = currentHoldTime / currentInteractable.interactionDuration;

            // Если полоса заполнилась полностью
            if (currentHoldTime >= currentInteractable.interactionDuration)
            {
                CompleteInteraction();
            }
        }
    }

    private void StartInteracting()
    {
        if (!isInteracting)
        {
            isInteracting = true;
            currentHoldTime = 0f;
            if (progressSlider != null)
            {
                progressSlider.gameObject.SetActive(true);
                progressSlider.value = 0f;
            }
        }
    }

    private void CompleteInteraction()
    {
        isInteracting = false;
        if (progressSlider != null) progressSlider.gameObject.SetActive(false);
        
        // Запоминаем ссылку, так как после TriggerInteraction объект может удалиться
        InteractableEntity completedObject = currentInteractable;
        
        // Если объект уничтожится, мы должны сбросить подсказки
        currentInteractable = null;
        if (hintObject != null) hintObject.SetActive(false);

        // Запускаем событие самого объекта
        completedObject.TriggerInteraction();
    }

    private void ResetInteraction()
    {
        isInteracting = false;
        currentHoldTime = 0f;
        if (progressSlider != null) progressSlider.gameObject.SetActive(false);
    }

    // Фиксируем, что игрок подошел к используемому объекту
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<InteractableEntity>(out var interactable))
        {
            currentInteractable = interactable;

            // Показываем подсказку и меняем в ней текст под конкретный объект
            if (hintObject != null)
            {
                hintObject.SetActive(true);
                var textComponent = hintObject.GetComponent<TMPro.TextMeshProUGUI>();
                if (textComponent != null) textComponent.text = interactable.hintText;
                else
                {
                    // На случай, если используется старый UI Text вместо TextMeshPro
                    var oldText = hintObject.GetComponent<Text>();
                    if (oldText != null) oldText.text = interactable.hintText;
                }
            }
        }
    }

    // Фиксируем, что игрок отошел от объекта
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<InteractableEntity>(out var interactable))
        {
            // Проверяем, что мы ушли именно от того объекта, который сейчас активен
            if (currentInteractable == interactable)
            {
                currentInteractable = null;
                ResetInteraction();
                if (hintObject != null) hintObject.SetActive(false);
            }
        }
    }
}