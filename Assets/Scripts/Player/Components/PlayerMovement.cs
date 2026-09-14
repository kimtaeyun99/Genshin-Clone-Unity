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

    private float verticalVelocity;
    public float VerticalVelocity => verticalVelocity;
    public bool IsGrounded => characterController.isGrounded;

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
        if(cameraTransform == null)
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

        if(moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        characterController.Move(moveDirection * speed * Time.deltaTime);

        Rotate(moveDirection);
    }
}
