using System;
using UnityEngine;

[Serializable]
public class CharacterRunTime
{
    private CharacterData data;
    private CharacterStat stat;

    private int level;
    private int ascensionPhase;
    private int maxHP;
    private int currentHP;
    private float currentEnergy;
    private float skillCoolDown;
    private float burstCoolDown;

    public event Action OnEnergyChanged;
    public event Action OnSkillCooldownChanged;
    public event Action OnBurstCooldownChanged;
    public event Action<int, int> OnHealthChanged;

    public float CurrentEnergy => currentEnergy;
    public float MaxEnergy => data.MaxEnergy;
    public float SkillCoolDown => skillCoolDown;
    public float BurstCoolDown => burstCoolDown;

    public bool IsSkillReady => skillCoolDown <= 0f;
    public bool IsBurstReady => burstCoolDown <= 0f && currentEnergy >= MaxEnergy;

    public CharacterData Data => data;
    public CharacterStat Stat => stat;
    public int Level => level;
    public int AscensionPhase => ascensionPhase;
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;
    public bool IsDead { get; private set; }

    public CharacterRunTime(CharacterData data)
    {
        this.data = data;
        level = 1;
        ascensionPhase = 0;
        maxHP = data.HP;
        currentHP = maxHP;
        currentEnergy = 0f;
        skillCoolDown = 0f;
        burstCoolDown = 0f;
    }

    public void SetStat(CharacterStat stat)
    {
        this.stat = stat;
    }

    public bool LevelUp()
    {
        int maxLevel = CharacterLevelRule.GetMaxLevel(ascensionPhase);
        if (level >= maxLevel)
            return false;

        level++;
        return true;
    }

    public bool Ascension()
    {
        if (ascensionPhase >= CharacterLevelRule.MaxLevels.Length - 1)
            return false;

        int maxLevel = CharacterLevelRule.GetMaxLevel(ascensionPhase);
        if (level < maxLevel)
            return false;

        ascensionPhase++;
        return true;
    }

    public void SetMaxHP(int newMaxHP)
    {
        int previousHP = currentHP;
        int previousMaxHP = maxHP;
        int hpShift = newMaxHP - maxHP;
        maxHP = newMaxHP;
        currentHP += hpShift;
        if (currentHP > maxHP)
            currentHP = maxHP;

        NotifyHealthChangedIfNeeded(previousHP, previousMaxHP);
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0)
            return;

        int previousHP = currentHP;
        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP;

        NotifyHealthChangedIfNeeded(previousHP, maxHP);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
            return;

        int previousHP = currentHP;
        currentHP -= Mathf.RoundToInt(damage);
        if (currentHP <= 0)
        {
            currentHP = 0;
            IsDead = true;
        }

        NotifyHealthChangedIfNeeded(previousHP, maxHP);
    }

    public void Revive()
    {
        // 전투 중 부활 기능은 없으며, 부활 시에는 HP를 완전히 회복한다.
        if (!IsDead)
            return;

        int previousHP = currentHP;
        IsDead = false;
        currentHP = maxHP;
        NotifyHealthChangedIfNeeded(previousHP, maxHP);
    }

    public void StartSkillCoolDown(float coolDown)
    {
        if (skillCoolDown == coolDown)
            return;

        skillCoolDown = coolDown;
        OnSkillCooldownChanged?.Invoke();
    }

    public void StartBurstCoolDown(float coolDown)
    {
        if (burstCoolDown == coolDown)
            return;

        burstCoolDown = coolDown;
        OnBurstCooldownChanged?.Invoke();
    }

    public void UpdateCoolDown(float deltaTime)
    {
        if (skillCoolDown > 0f)
        {
            float oldCooldown = skillCoolDown;
            skillCoolDown -= deltaTime;
            if (skillCoolDown < 0f)
                skillCoolDown = 0f;

            if (skillCoolDown != oldCooldown)
                OnSkillCooldownChanged?.Invoke();
        }

        if (burstCoolDown > 0f)
        {
            float oldCooldown = burstCoolDown;
            burstCoolDown -= deltaTime;
            if (burstCoolDown < 0f)
                burstCoolDown = 0f;

            if (burstCoolDown != oldCooldown)
                OnBurstCooldownChanged?.Invoke();
        }
    }

    public void AddEnergy(float amount)
    {
        if (amount <= 0f)
            return;

        float newEnergy = Mathf.Min(currentEnergy + amount, MaxEnergy);
        if (newEnergy == currentEnergy)
            return;

        currentEnergy = newEnergy;
        OnEnergyChanged?.Invoke();
    }

    public bool ConsumeBurstEnergy()
    {
        if (currentEnergy < MaxEnergy)
            return false;

        bool energyChanged = currentEnergy != 0f;
        currentEnergy = 0f;
        if (energyChanged)
            OnEnergyChanged?.Invoke();
        return true;
    }

    public void Respawn()
    {
        int previousHP = currentHP;
        int previousMaxHP = maxHP;
        bool energyChanged = currentEnergy != 0f;
        bool skillChanged = skillCoolDown != 0f;
        bool burstChanged = burstCoolDown != 0f;

        IsDead = false;
        currentHP = maxHP;
        currentEnergy = 0f;
        skillCoolDown = 0f;
        burstCoolDown = 0f;

        NotifyHealthChangedIfNeeded(previousHP, previousMaxHP);
        if (energyChanged) OnEnergyChanged?.Invoke();
        if (skillChanged) OnSkillCooldownChanged?.Invoke();
        if (burstChanged) OnBurstCooldownChanged?.Invoke();
    }

    public void LoadGrowthData(int level, int ascensionPhase)
    {
        this.level = level;
        this.ascensionPhase = ascensionPhase;
    }

    public void LoadStateData(int currentHP, float currentEnergy, bool isDead)
    {
        int previousHP = this.currentHP;
        int previousMaxHP = maxHP;
        this.currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        float newEnergy = Mathf.Clamp(currentEnergy, 0f, MaxEnergy);
        bool energyChanged = this.currentEnergy != newEnergy;
        this.currentEnergy = newEnergy;

        // 기존 저장 파일의 사망 상태와 HP 값은 이번 작업에서 임의 보정하지 않는다.
        // 기존 LoadStateData 규칙 유지. 저장 데이터 검증은 SaveManager 단계에서 진행.
        IsDead = isDead;

        NotifyHealthChangedIfNeeded(previousHP, previousMaxHP);
        if (energyChanged)
            OnEnergyChanged?.Invoke();
    }

    private void NotifyHealthChangedIfNeeded(int previousHP, int previousMaxHP)
    {
        if (currentHP == previousHP && maxHP == previousMaxHP)
            return;

        OnHealthChanged?.Invoke(currentHP, maxHP);
    }
}
