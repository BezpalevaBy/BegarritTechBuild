using System.Collections.Generic;
using UnityEngine;

// Типы угроз для анализа
public enum ThreatType
{
    MeleeCombat,   // Ближний бой (пропущенные удары вблизи)
    RangedCombat,  // Дальний бой (стрелы, магические снаряды)
    Traps,         // Ловушки (шипы, огонь, циркулярные пилы)
    Speedrun       // Скорость прохождения (время на комнату)
}

// Причина получения урона / неудачи
public enum DamageSourceContext
{
    DirectHit,           // Получил урон «в лицо» (не успел среагировать)
    FailedBlockOrParry,  // Пытался заблокировать, но пробил щит / не попал в тайминг
    TrapTriggered        // Наступил на ловушку
}

[System.Serializable]
public class PlayerSkillProfile
{
    public Dictionary<ThreatType, float> DamageTakenByThreat = new Dictionary<ThreatType, float>();
    public int successfulBlocks = 0;
    public int failedBlocks = 0;
    public float floorStartTime;

    public void Reset()
    {
        DamageTakenByThreat[ThreatType.MeleeCombat] = 0;
        DamageTakenByThreat[ThreatType.RangedCombat] = 0;
        DamageTakenByThreat[ThreatType.Traps] = 0;
        DamageTakenByThreat[ThreatType.Speedrun] = 0;
        successfulBlocks = 0;
        failedBlocks = 0;
        floorStartTime = Time.time;
    }
}