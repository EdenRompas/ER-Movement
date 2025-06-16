using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    public static event Action<Vector2> OnMoveInput;
    public static event Action OnJumpInput;
    public static event Action<bool> OnRunningInput;
    public static event Action<Vector2> OnLookInput;
    public static event Action OnChangeCameraInput;

    [SerializeField] private InputActionReference _movementButtonInput;
    [SerializeField] private InputActionReference _jumpButtonInput;
    [SerializeField] private InputActionReference _runningButtonInput;
    [SerializeField] private InputActionReference _lookXInput;
    [SerializeField] private InputActionReference _lookYInput;
    [SerializeField] private InputActionReference _changeCameraInput;

    private void OnEnable()
    {
        _jumpButtonInput.action.performed += SendJumpInput;
        _runningButtonInput.action.performed += ctx => SendRunningInput(true);
        _runningButtonInput.action.canceled += ctx => SendRunningInput(false);
        _changeCameraInput.action.performed += SendChangeCameraInput;
    }

    private void OnDisable()
    {
        _jumpButtonInput.action.performed -= SendJumpInput;
        _runningButtonInput.action.performed -= ctx => SendRunningInput();
        _runningButtonInput.action.canceled -= ctx => SendRunningInput();
        _changeCameraInput.action.performed -= SendChangeCameraInput;
    }

    private void Update()
    {
        SendMoveInput();
        SendLookInput();
    }

    private void SendMoveInput()
    {
        Vector2 movement = _movementButtonInput.action.ReadValue<Vector2>();
        OnMoveInput?.Invoke(movement);
    }

    private void SendJumpInput(InputAction.CallbackContext context)
    {
        OnJumpInput?.Invoke();
    }

    private void SendRunningInput(bool isRunning = false)
    {
        OnRunningInput?.Invoke(isRunning);
    }

    private void SendLookInput()
    {
        Vector2 look = new Vector2(_lookXInput.action.ReadValue<float>(), _lookYInput.action.ReadValue<float>());
        OnLookInput?.Invoke(look);
    }

    private void SendChangeCameraInput(InputAction.CallbackContext context)
    {
        OnChangeCameraInput?.Invoke();
    }
}