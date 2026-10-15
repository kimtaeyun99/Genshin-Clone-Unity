using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    // 연속 입력
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    // 상태 입력
    public bool IsSprintHeld { get; private set; }

    // 일회성 입력
    public event Action OnAttack;
    public event Action OnInteract;
    public event Action OnJump;
    public event Action OnDodge;
    public event Action OnSkill;
    public event Action OnBurst;
    public event Action OnMenu;
    public event Action OnInventory;
    public event Action OnToggle;


    // 캐릭터 변경
    public event Action<int> OnSwitchCharacter;

    private PlayerInputActions inputActions;
    public bool CanPlayerInput =>
    Cursor.lockState == CursorLockMode.Locked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        // Move / Look
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;

        // Movement
        inputActions.Player.Jump.performed += OnJumpPerformed;

        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;

        // Combat
        inputActions.Player.Attack.performed += OnAttackPerformed;
        inputActions.Player.Skill.performed += OnSkillPerformed;
        inputActions.Player.Burst.performed += OnBurstPerformed;

        // Interaction
        inputActions.Player.Interact.performed += OnInteractPerformed;

        // UI
        inputActions.Player.Menu.performed += OnMenuPerformed;
        inputActions.Player.Inventory.performed += OnInventoryPerformed;

        // Character Switch
        inputActions.Player.Switch1.performed += OnSwitch1;
        inputActions.Player.Switch2.performed += OnSwitch2;
        inputActions.Player.Switch3.performed += OnSwitch3;
        inputActions.Player.Switch4.performed += OnSwitch4;

        inputActions.Player.Toggle.performed += OnTogglePerformed;
    }

    private void OnDisable()
    {
        MoveInput = Vector2.zero;
        LookInput = Vector2.zero;
        IsSprintHeld = false;

        // Move / Look
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled -= OnLook;

        // Movement
        inputActions.Player.Jump.performed -= OnJumpPerformed;

        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;

        // Combat
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Player.Skill.performed -= OnSkillPerformed;
        inputActions.Player.Burst.performed -= OnBurstPerformed;

        // Interaction
        inputActions.Player.Interact.performed -= OnInteractPerformed;

        // UI
        inputActions.Player.Menu.performed -= OnMenuPerformed;
        inputActions.Player.Inventory.performed -= OnInventoryPerformed;

        // Character Switch
        inputActions.Player.Switch1.performed -= OnSwitch1;
        inputActions.Player.Switch2.performed -= OnSwitch2;
        inputActions.Player.Switch3.performed -= OnSwitch3;
        inputActions.Player.Switch4.performed -= OnSwitch4;

        inputActions.Player.Toggle.performed -= OnTogglePerformed;

        inputActions.Disable();
    }

    // =========================
    // Move / Look
    // =========================

    private void OnMove(InputAction.CallbackContext context)
    {
        if (!CanPlayerInput)
        {
            MoveInput = Vector2.zero;
            return;
        }

        MoveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    // =========================
    // Movement
    // =========================

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (!CanPlayerInput)
        {
            return;
        }

        OnJump?.Invoke();
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            IsSprintHeld = false;
            return;
        }

        if (!CanPlayerInput)
            return;

        if (context.performed)
        {
            IsSprintHeld = true;
            OnDodge?.Invoke();
        }
    }

    // =========================
    // Combat
    // =========================

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!CanPlayerInput)
        {
            return;
        }

        OnAttack?.Invoke();
    }

    private void OnSkillPerformed(InputAction.CallbackContext context)
    {
        if (!CanPlayerInput)
        {
            return;
        }

        OnSkill?.Invoke();
    }

    private void OnBurstPerformed(InputAction.CallbackContext context)
    {
        if (!CanPlayerInput)
        {
            return;
        }

        OnBurst?.Invoke();
    }

    // =========================
    // Interaction
    // =========================

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        OnInteract?.Invoke();
    }

    // =========================
    // UI
    // =========================

    private void OnMenuPerformed(InputAction.CallbackContext context)
    {
        OnMenu?.Invoke();
    }

    private void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        OnInventory?.Invoke();
    }

    // =========================
    // Character Switch
    // =========================

    private void OnSwitch1(InputAction.CallbackContext context)
    {
        if (!CanPlayerInput)
        {
            return;
        }
        OnSwitchCharacter?.Invoke(1);
    }

    private void OnSwitch2(InputAction.CallbackContext context)
    {
        if (!CanPlayerInput)
        {
            return;
        }
        OnSwitchCharacter?.Invoke(2);
    }

    private void OnSwitch3(InputAction.CallbackContext context)
    {
        if (!CanPlayerInput)
        {
            return;
        }
        OnSwitchCharacter?.Invoke(3);
    }

    private void OnSwitch4(InputAction.CallbackContext context)
    {
        if (!CanPlayerInput)
        {
            return;
        }
        OnSwitchCharacter?.Invoke(4);
    }
    private void OnTogglePerformed(InputAction.CallbackContext context)
    {

        OnToggle?.Invoke();
    }
}