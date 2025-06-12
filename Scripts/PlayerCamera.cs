using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private enum CameraType
    {
        TPS,
        FPS
    }

    [SerializeField] private CameraType _cameraType;

    [SerializeField] private GameObject _cameraTPS;
    [SerializeField] private Transform _orientation;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private GameObject _playerModel;

    [SerializeField] private GameObject _cameraFPS;
    [SerializeField] private float _sensitivity;
    [SerializeField] private float _maxLookAngle;

    private Vector2 _inputLook;
    private float _yaw;
    private float _pitch;

    private void OnEnable()
    {
        PlayerController.OnLookInput += SetInputLook;
        PlayerController.OnChangeCameraInput += SetCamera;
    }

    private void OnDisable()
    {
        PlayerController.OnLookInput -= SetInputLook;
        PlayerController.OnChangeCameraInput -= SetCamera;
    }

    private void Start()
    {
        switch (_cameraType)
        {
            case CameraType.TPS:

                _cameraTPS.SetActive(true);
                _cameraFPS.SetActive(false);
                _playerModel.SetActive(true);

                break;

            case CameraType.FPS:

                _cameraFPS.SetActive(true);
                _cameraTPS.SetActive(false);
                _playerModel.SetActive(false);

                break;
        }
    }

    private void Update()
    {
        if (_cameraType == CameraType.FPS)
        {
            if (Camera.main.transform.position == _cameraFPS.transform.position && _playerModel.activeSelf == true)
            {
                _playerModel.SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        switch(_cameraType)
        {
            case CameraType.TPS:

                TPSHandle();

                break;

            case CameraType.FPS:

                FPSHandle();

                break;
        }
    }

    private void TPSHandle()
    {
        Vector3 viewDirection = _player.position - new Vector3(_cameraTransform.position.x, _player.position.y, _cameraTransform.position.z);
        _orientation.forward = viewDirection.normalized;
    }

    private void FPSHandle() 
    {
        float lookX = _inputLook.x * 0.1f;
        float lookY = _inputLook.y * 0.1f;

        _yaw = transform.localEulerAngles.y + lookX * _sensitivity;
        _pitch -= _sensitivity * lookY;

        _pitch = Mathf.Clamp(_pitch, -_maxLookAngle, _maxLookAngle);

        transform.localEulerAngles = new Vector3(0, _yaw, 0);
        _cameraFPS.transform.localEulerAngles = new Vector3(_pitch, 0, 0);
    }

    private void SetInputLook(Vector2 input)
    {
        _inputLook = input;
    }

    private void SetCamera()
    {
        if (_cameraType != CameraType.TPS)
        {
            _cameraTPS.SetActive(true);
            _cameraFPS.SetActive(false);
            _playerModel.SetActive(true);

            _cameraType = CameraType.TPS;
        }
        else
        {
            _cameraFPS.SetActive(true);
            _cameraTPS.SetActive(false);

            _cameraType = CameraType.FPS;
        }
    }
}