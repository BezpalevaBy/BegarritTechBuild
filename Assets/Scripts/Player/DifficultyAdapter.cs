using UnityEngine;

public class DifficultyAdapter : MonoBehaviour
{
    public static DifficultyAdapter Instance { get; private set; }

    public enum DdaPhase { Testing, Hardening, Training }
    
    [Header("Текущее состояние DDA")]
    [SerializeField] private DdaPhase currentPhase = DdaPhase.Testing;
    
    public PlayerSkillProfile TargetProfile { get; private set; } = new PlayerSkillProfile();

    // Минимальный порог урона, чтобы считать тип угрозы "проблемным"
    private const float WeaknessDamageThreshold = 40f; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            TargetProfile.Reset();
        }
        else { Destroy(gameObject); }
    }

    // ==========================================================================================
    // ВХОДЯЩИЕ МЕТРИКИ (Вызываются из PlayerHealth, PlayerBlock, DamageDealer)
    // ==========================================================================================

    /// <summary>
    /// Регистрирует любой урон, полученный игроком, с контекстом.
    /// </summary>
    public void TrackPlayerDamage(ThreatType threat, DamageSourceContext context, float amount)
    {
        if (!TargetProfile.DamageTakenByThreat.ContainsKey(threat))
            TargetProfile.DamageTakenByThreat[threat] = 0;

        TargetProfile.DamageTakenByThreat[threat] += amount;

        if (context == DamageSourceContext.FailedBlockOrParry)
        {
            TargetProfile.failedBlocks++;
        }
        
        Debug.Log($"[DDA Tracker] Игрок получил {amount} урона от {threat} ({context}).");
    }

    /// <summary>
    /// Регистрирует успешное отражение атаки (блок/парирование).
    /// </summary>
    public void TrackSuccessfulBlock()
    {
        TargetProfile.successfulBlocks++;
    }

    // ==========================================================================================
    // АНАЛИЗ И ВЫБОР СИТУАЦИИ
    // ==========================================================================================

    /// <summary>
    /// На основе собранной статистики решает, какой тип спавна (ситуацию) предоставить игроку.
    /// </summary>
    public EncounterDecision EvaluateNextEncounter()
    {
        ThreatType mainWeakness = ThreatType.Speedrun;
        float maxDamage = 0;

        // Ищем, от чего игрок страдает больше всего
        foreach (var kvp in TargetProfile.DamageTakenByThreat)
        {
            if (kvp.Value > maxDamage)
            {
                maxDamage = kvp.Value;
                mainWeakness = kvp.Key;
            }
        }

        // Проверяем защиту отдельно
        bool hasShieldIssues = TargetProfile.failedBlocks > 0 && 
                               (float)TargetProfile.successfulBlocks / (TargetProfile.successfulBlocks + TargetProfile.failedBlocks) < 0.5f;

        // Ограничение по времени (если копается слишком долго)
        float timeSpent = Time.time - TargetProfile.floorStartTime;
        bool isTooSlow = timeSpent > 300f; // например, больше 5 минут на этаж

        // МЕНЕДЖЕР СОСТОЯНИЙ (Фазы)
        if (maxDamage == 0 && !isTooSlow)
        {
            // У игрока всё отлично -> Фаза Усиления (Hardening)
            currentPhase = DdaPhase.Hardening;
            return new EncounterDecision(currentPhase, GetRandomThreat(), "Игрок доминирует. Усиливаем общий натиск.");
        }
        
        if (maxDamage > WeaknessDamageThreshold || hasShieldIssues || isTooSlow)
        {
            // Обнаружена слабость -> Фаза Тренировки / Прививки (Training)
            currentPhase = DdaPhase.Training;
            
            ThreatType trainingThreat = mainWeakness;
            if (hasShieldIssues && mainWeakness == ThreatType.MeleeCombat) 
                trainingThreat = ThreatType.MeleeCombat; // Будем учить закрываться щитом

            return new EncounterDecision(currentPhase, trainingThreat, $"Выявлена слабость: {trainingThreat}. Спавним тренировочную ситуацию.");
        }

        // По умолчанию — продолжаем тестирование
        currentPhase = DdaPhase.Testing;
        return new EncounterDecision(currentPhase, GetRandomThreat(), "Стадия тестов: смешанные угрозы.");
    }

    private ThreatType GetRandomThreat()
    {
        System.Array values = System.Enum.GetValues(typeof(ThreatType));
        return (ThreatType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    public void ResetFloorMetrics() => TargetProfile.Reset();
}

// Контейнер для передачи решения режиссеру спавна
public struct EncounterDecision
{
    public DifficultyAdapter.DdaPhase Phase;
    public ThreatType FocusThreat;
    public string Reason;

    public EncounterDecision(DifficultyAdapter.DdaPhase phase, ThreatType threat, string reason)
    {
        Phase = phase;
        FocusThreat = threat;
        Reason = reason;
    }
}