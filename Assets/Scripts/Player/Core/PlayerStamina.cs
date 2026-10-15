using System;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;

    [Header("Consume")]
    [SerializeField] private float dashStaminaCostPerSecond = 15f;
    [SerializeField] private float dodgeStaminaCost = 20f;

    [Header("Recovery")]
    [SerializeField] private float recoveryPerSecond = 20f;
    [SerializeField] private float recoveryDelay = 1f;

    [SerializeField] private float actionLimitPercent = 0.2f;

    public float CurrentStamina { get; private set; }
    private float recoverTimer;
    public bool CanUseStaminaAction => CurrentStamina >= MaxStamina * actionLimitPercent;
    public float MaxStamina => maxStamina;
    public float DashStaminaCostPerSecond => dashStaminaCostPerSecond;
    public float DodgeStaminaCost => dodgeStaminaCost;
    public bool IsEmpty => CurrentStamina <= 0f;
    public bool IsFull => CurrentStamina >= maxStamina;

    public event Action<float, float> OnStaminaChanged;

    private void Awake()
    {
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        RecoveryStamina();
    }

    public void ConsumeStamina(float staminaCost)
    {
        SetStamina(CurrentStamina - staminaCost * Time.deltaTime);
        recoverTimer = 0f;
    }

    public void ConsumeStaminaInstant(float staminaCost)
    {
        SetStamina(CurrentStamina - staminaCost);
        recoverTimer = 0f;
    }

    public void RecoveryStamina()
    {
        if (IsFull)
        {
            return;
        }

        recoverTimer += Time.deltaTime;

        if (recoveryDelay > recoverTimer)
        {
            return;
        }

        SetStamina(CurrentStamina + recoveryPerSecond * Time.deltaTime);
    }

    private void SetStamina(float value)
    {
        float clampedValue = Mathf.Clamp(value, 0f, maxStamina);
        if (CurrentStamina == clampedValue)
        {
            return;
        }

        CurrentStamina = clampedValue;
        OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
    }
}
