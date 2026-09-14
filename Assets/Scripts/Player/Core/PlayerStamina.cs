using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [Header("Consume")]
    [SerializeField] private float dashStaminaCostPerSecond = 15f;
    [Header("Recovery")]
    [SerializeField] private float recoveryPerSecond = 20f;
    [SerializeField] private float recoveryDelay = 1f;
    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;
    public float DashStaminaCostPerSecond => dashStaminaCostPerSecond;
    public bool IsEmpty => CurrentStamina <= 0f;
    public bool IsFull => CurrentStamina >= maxStamina;

    private float recoverTimer;

    private void Awake()
    {
        CurrentStamina = maxStamina;
    }
    private void Update()
    {
        RecoveryStamina();
    }
    public void ConsumeStamina(float StaminaCost)
    {
        CurrentStamina -= StaminaCost * Time.deltaTime;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f,maxStamina);
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

        CurrentStamina += recoveryPerSecond * Time.deltaTime;

        CurrentStamina = Mathf.Min(CurrentStamina, maxStamina);
    }

}
