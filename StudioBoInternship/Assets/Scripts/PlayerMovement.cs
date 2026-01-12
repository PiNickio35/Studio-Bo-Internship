using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float movementSpeed = 5f;
    private Vector2 _movementInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rb.linearVelocity = _movementInput * movementSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }
}
