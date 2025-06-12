using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Action OnIdle {  get; set; }
    public Action OnWalking { get; set; }
    public Action OnSprinting { get; set; }
    public Action OnJumping { get; set; }
    public Action OnJumpWhileMoving { get; set; }
    public bool IsWalking { get; private set; }
    public bool IsSprinting { get; private set; }

    public enum MovementType
    {
        TPS,
        FPS,
    }

    [SerializeField] private MovementType _movementType;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _orientation;
    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _sprintSpeed = 8;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private float _directionSmoothTime = 0.1f;

    private Vector2 _inputDirection;
    private Vector3 _playerVelocity;

    private Vector3 _smoothedRotation;
    private Vector3 _directionVelocity;

    private float _currentVelocity;
    private float _smoothedAngle;

    private bool _isJumping;
    private bool _isSprinting;
    private bool _isNotAllowedSprinting;

    private void OnEnable()
    {
        PlayerController.OnMoveInput += SetInputDirection;
        PlayerController.OnJumpInput += SetJump;
        PlayerController.OnSprintInput += SetSprint;
        PlayerController.OnChangeCameraInput += SetCamera;
    }

    private void OnDisable()
    {
        PlayerController.OnMoveInput -= SetInputDirection;
        PlayerController.OnJumpInput -= SetJump;
        PlayerController.OnSprintInput -= SetSprint;
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

    public void ModifyPlayerSpeed(int speedModify, bool isHoldSprint)
    {
        _movementSpeed += speedModify;
        _isNotAllowedSprinting = isHoldSprint;
    }

    private void TPSMovement()
    {
        Vector3 moveDirection = (_orientation.forward * _inputDirection.y + _orientation.right * _inputDirection.x).normalized;

        float currentSpeed;

        if (!_isNotAllowedSprinting)
        {
            currentSpeed = _isSprinting ? _sprintSpeed : _movementSpeed;
        }
        else
        {
            currentSpeed = _movementSpeed;
        }

        if (_isJumping && _characterController.isGrounded)
        {
            _playerVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

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

            if (_characterController.isGrounded && _isSprinting)
            {
                OnSprinting?.Invoke();

                IsSprinting = true;
                IsWalking = false;
            }
            else if (_characterController.isGrounded)
            {
                OnWalking?.Invoke();

                IsWalking = true;
                IsSprinting = false;
            }
        }
        else if (_characterController.isGrounded)
        {
            OnIdle?.Invoke();

            IsWalking = false;
            IsSprinting = false;
        }
    }

    private void FPSMovement()
    {
        float currentSpeed = _isSprinting ? _sprintSpeed : _movementSpeed;

        Vector3 moveDirection = transform.TransformDirection(new Vector3(_inputDirection.x, 0, _inputDirection.y));

        if (_isJumping && _characterController.isGrounded) 
        {
            _playerVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

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
            if (_characterController.isGrounded && _isSprinting)
            {
                OnSprinting?.Invoke();

                IsSprinting = true;
                IsWalking = false;
            }
            else if (_characterController.isGrounded)
            {
                OnWalking?.Invoke();

                IsWalking = true;
                IsSprinting = false;
            }
        }
        else if (_characterController.isGrounded)
        {
            OnIdle?.Invoke();

            IsWalking = false;
            IsSprinting = false;
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

    private void SetSprint(bool isSprinting)
    {
        _isSprinting = isSprinting;
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