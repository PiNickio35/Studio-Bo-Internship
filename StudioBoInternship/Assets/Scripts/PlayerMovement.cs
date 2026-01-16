using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    [SerializeField] private float movementSpeed = 5f;
    private Vector2 _movementInput;
    private Vector3 _currentPosition, _lastPosition;
    private bool _playingFootsteps = false;
    public float footstepSpeed = 0.5f;

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
        if (PauseController.IsGamePaused)
        {
            _rb.linearVelocity = Vector2.zero;
            _animator.SetBool("isWalking", false);
            StopFootSteps();
            return;
        }
        _rb.linearVelocity = _movementInput * movementSpeed;
        _animator.SetBool("isWalking", _rb.linearVelocity.magnitude > 0);
        if (_rb.linearVelocity.magnitude > 0 && !_playingFootsteps)
        {
            PlayFootStep();
        }
        else if (_rb.linearVelocity.magnitude == 0)
        {
            StopFootSteps();
        }
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

    private void StartFootSteps()
    {
        _playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootStep), 0, footstepSpeed);
    }

    private void StopFootSteps()
    {
        _playingFootsteps = false;
        CancelInvoke(nameof(PlayFootStep));
    }

    private void PlayFootStep()
    {
        SoundEffectManager.Play("Footsteps", true);
    }
}
