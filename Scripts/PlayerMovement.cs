using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Action OnIdle {  get; set; }
    public Action OnWalking { get; set; }
    public Action OnRunning { get; set; }
    public Action OnJumping { get; set; }
    public Action OnJumpWhileMoving { get; set; }
    public Action OnLanding { get; set; }
    public bool IsWalking { get; private set; }
    public bool IsRunning { get; private set; }

    public enum MovementType
    {
        TPS,
        FPS,
    }

    [SerializeField] private MovementType _movementType;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private SO_Player _playerSO;
    [SerializeField] private Transform _orientation;

    private Vector2 _inputDirection;
    private Vector3 _playerVelocity;

    private Vector3 _smoothedRotation;
    private Vector3 _directionVelocity;

    private float _gravity = -9.81f;
    private float _directionSmoothTime = 0.1f;

    private float _currentWalkingSpeed;
    private float _currentVelocity;
    private float _smoothedAngle;

    private bool _isJumping;
    private bool _isRunning;
    private bool _isAllowedRunning = true;
    private bool _isGoingToLanding;

    private void Start()
    {
        _currentWalkingSpeed = _playerSO.WalkingSpeed;
    }

    private void OnEnable()
    {
        PlayerController.OnMoveInput += SetInputDirection;
        PlayerController.OnJumpInput += SetJump;
        PlayerController.OnRunningInput += SetRunning;
        PlayerController.OnChangeCameraInput += SetCamera;
    }

    private void OnDisable()
    {
        PlayerController.OnMoveInput -= SetInputDirection;
        PlayerController.OnJumpInput -= SetJump;
        PlayerController.OnRunningInput -= SetRunning;
        PlayerController.OnChangeCameraInput -= SetCamera;
    }

    private void Update()
    {
        switch(_movementType)
        {
            case MovementType.TPS:

                TPSMovement();

                break;

            case MovementType.FPS:

                FPSMovement();

                break;
        }
    }

    public void ModifyPlayerSpeed(int speedModify, bool isHoldRunning, bool isSetDefault = false)
    {
        if (isSetDefault)
        {
            _currentWalkingSpeed = _playerSO.WalkingSpeed;
            _isAllowedRunning = isHoldRunning;
        }
        else
        {
            _currentWalkingSpeed += speedModify;
            _isAllowedRunning = isHoldRunning;
        }
    }

    private void TPSMovement()
    {
        Vector3 moveDirection = (_orientation.forward * _inputDirection.y + _orientation.right * _inputDirection.x).normalized;

        float currentSpeed;

        if (_isAllowedRunning)
        {
            currentSpeed = _isRunning ? _playerSO.RunningSpeed : _currentWalkingSpeed;
        }
        else
        {
            currentSpeed = _currentWalkingSpeed;
        }

        if (_isJumping && _characterController.isGrounded)
        {
            _playerVelocity.y = Mathf.Sqrt(_playerSO.JumpHeight * -2f * _gravity);

            if (moveDirection.magnitude > 0.1f)
            {
                OnJumpWhileMoving?.Invoke();
            }
            else
            {
                OnJumping?.Invoke();
            }

            _isJumping = false;
            _isGoingToLanding = true;
        }
        else if (_characterController.isGrounded)
        {
            _playerVelocity.y = -1;
        }
        else
        {
            _playerVelocity.y += _gravity * Time.deltaTime;
        }

        _characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
        _characterController.Move(_playerVelocity * Time.deltaTime);

        _smoothedRotation = Vector3.SmoothDamp(_smoothedRotation, moveDirection, ref _directionVelocity, _directionSmoothTime);

        if (moveDirection.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

            _smoothedAngle = Mathf.SmoothDampAngle(
                _smoothedAngle,
                targetAngle,
                ref _currentVelocity,
                _directionSmoothTime
            );

            Quaternion rotation = Quaternion.Euler(0f, _smoothedAngle, 0f);
            transform.rotation = rotation;

            if (_characterController.isGrounded && _isRunning)
            {
                OnRunning?.Invoke();

                IsRunning = true;
                IsWalking = false;
            }
            else if (_characterController.isGrounded)
            {
                OnWalking?.Invoke();

                IsWalking = true;
                IsRunning = false;
            }
        }
        else if (_characterController.isGrounded)
        {
            OnIdle?.Invoke();

            IsWalking = false;
            IsRunning = false;
        }

        if (_isGoingToLanding && _characterController.isGrounded)
        {
            _isGoingToLanding = false;
            OnLanding?.Invoke();
        }
    }

    private void FPSMovement()
    {
        float currentSpeed = _isRunning ? _playerSO.RunningSpeed : _currentWalkingSpeed;

        Vector3 moveDirection = transform.TransformDirection(new Vector3(_inputDirection.x, 0, _inputDirection.y));

        if (_isJumping && _characterController.isGrounded) 
        {
            _playerVelocity.y = Mathf.Sqrt(_playerSO.JumpHeight * -2f * _gravity);

            if (moveDirection.magnitude > 0.1f)
            {
                OnJumpWhileMoving?.Invoke();
            }
            else
            {
                OnJumping?.Invoke();
            }

            _isJumping = false;
        }
        else if (_characterController.isGrounded)
        {
            _playerVelocity.y = -1;
        }
        else
        {
            _playerVelocity.y += _gravity * Time.deltaTime;
        }

        _characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
        _characterController.Move(_playerVelocity * Time.deltaTime);

        if (moveDirection.magnitude > 0.1f)
        {
            if (_characterController.isGrounded && _isRunning)
            {
                OnRunning?.Invoke();

                IsRunning = true;
                IsWalking = false;
            }
            else if (_characterController.isGrounded)
            {
                OnWalking?.Invoke();

                IsWalking = true;
                IsRunning = false;
            }
        }
        else if (_characterController.isGrounded)
        {
            OnIdle?.Invoke();

            IsWalking = false;
            IsRunning = false;
        }
    }

    private void SetInputDirection(Vector2 input)
    {
        _inputDirection = input;
    }

    private void SetJump()
    {
        _isJumping = true;
    }

    private void SetRunning(bool isRunning)
    {
        _isRunning = isRunning;
    }

    private void SetCamera()
    {
        if (_movementType != MovementType.TPS)
        {
            _movementType = MovementType.TPS;
        }
        else
        {
            _movementType = MovementType.FPS;
        }
    }
}