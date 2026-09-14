using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedVelocity = -2f;
    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [Header("Air Movement")]
    [SerializeField] private float airMoveSpeed = 3f;
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float groundCheckRadius = 0.3f;

    public bool IsGrounded { get; private set; }
    private float verticalVelocity;
    public float VerticalVelocity => verticalVelocity;

    private CharacterController characterController;
    private Transform cameraTransform;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }
    private void Update()
    {
        CheckGround();
    }
    public void Move(Vector2 input)
    {
        MoveInternal(input, moveSpeed);
    }

    private void Rotate(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude <= 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(moveDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
    public void ApplyGravity()
    {
        if (IsGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedVelocity;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        characterController.Move(
            Vector3.up * verticalVelocity * Time.deltaTime
        );
    }
    public void CheckGround()
    {
        Vector3 origin = transform.position + characterController.center + Vector3.down * (characterController.height * 0.5f - characterController.radius);

        IsGrounded = Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);
    }
    public void Jump()
    {
        verticalVelocity = Mathf.Sqrt(
            jumpHeight * -2f * gravity
        );
    }
    public void AirMove(Vector2 input)
    {
        MoveInternal(input, airMoveSpeed);
    }
    private void MoveInternal(Vector2 input, float speed)
    {
        if (cameraTransform == null)
        {
            return;
        }
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * input.y + right * input.x;

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        characterController.Move(moveDirection * speed * Time.deltaTime);

        Rotate(moveDirection);
    }
    private void OnDrawGizmosSelected()
    {
        CharacterController controller = GetComponent<CharacterController>();

        if (controller == null)
            return;

        Vector3 origin =
            transform.position +
            controller.center +
            Vector3.down * (controller.height * 0.5f - controller.radius);

        Gizmos.DrawWireSphere(
            origin + Vector3.down * groundCheckDistance,
            groundCheckRadius
        );
    }
}
