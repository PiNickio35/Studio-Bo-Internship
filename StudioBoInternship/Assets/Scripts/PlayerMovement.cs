using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    [SerializeField] private float movementSpeed = 5f;
    private Vector2 _movementInput;
    private Vector3 _currentPosition, _lastPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.gameObject.GetComponent<SaveController>().LoadGame();
        GameManager.Instance.nextSpawnPoint = "";
        GameManager.Instance.lastPlayerPosition = Vector3.zero;
    }

    private void Update()
    {
        _rb.linearVelocity = _movementInput * movementSpeed;
        _currentPosition = transform.position;
        if (_currentPosition == _lastPosition)
        {
            GameManager.Instance.isWalking = false;
        }
        else
        {
            GameManager.Instance.isWalking = true;
        }
        _lastPosition = _currentPosition;
    }

    public void Move(InputAction.CallbackContext context)
    {
        _animator.SetBool("isWalking", true);
        if (context.canceled)
        {
            _animator.SetBool("isWalking", false);
            _animator.SetFloat("LastInputX", _movementInput.x);
            _animator.SetFloat("LastInputY", _movementInput.y);
        }
        _movementInput = context.ReadValue<Vector2>();
        _animator.SetFloat("InputX", _movementInput.x);
        _animator.SetFloat("InputY", _movementInput.y);
    }
}
