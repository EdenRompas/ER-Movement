using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    public static event Action<Vector2> OnMoveInput;
    public static event Action OnJumpInput;
    public static event Action<bool> OnSprintInput;
    public static event Action<Vector2> OnLookInput;
    public static event Action OnChangeCameraInput;

    [SerializeField] private InputActionReference _movementButtonInput;
    [SerializeField] private InputActionReference _jumpButtonInput;
    [SerializeField] private InputActionReference _sprintButtonInput;
    [SerializeField] private InputActionReference _lookXInput;
    [SerializeField] private InputActionReference _lookYInput;
    [SerializeField] private InputActionReference _changeCameraInput;

    private void OnEnable()
    {
        _jumpButtonInput.action.performed += SendJumpInput;
        _sprintButtonInput.action.performed += ctx => SendSprintInput(true);
        _sprintButtonInput.action.canceled += ctx => SendSprintInput(false);
        _changeCameraInput.action.performed += SendChangeCameraInput;
    }

    private void OnDisable()
    {
        _jumpButtonInput.action.performed -= SendJumpInput;
        _sprintButtonInput.action.performed -= ctx => SendSprintInput();
        _sprintButtonInput.action.canceled -= ctx => SendSprintInput();
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

    private void SendSprintInput(bool isSprint = false)
    {
        OnSprintInput?.Invoke(isSprint);
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