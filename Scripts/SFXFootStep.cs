using UnityEngine;

public class SFXFootStep : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private SO_Player _playerSO;
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private float _volume;
    [SerializeField] private float _walkingInterval = 0.8f;
    [SerializeField] private float _sprintingInterval = 0.3f;

    private float _stepTimer;

    private void OnEnable()
    {
        _playerMovement.OnLanding += PlayLandingSound;
    }

    private void OnDisable()
    {
        _playerMovement.OnLanding -= PlayLandingSound;
    }

    private void Update()
    {
        if ((_playerMovement.IsWalking || _playerMovement.IsRunning) && _characterController.isGrounded)
        {
            _stepTimer -= Time.deltaTime;

            if (_stepTimer <= 0f)
            {
                if (_playerMovement.IsWalking)
                {
                    PlayFootStepSound();

                    _stepTimer = _walkingInterval;
                }
                else if (_playerMovement.IsRunning)
                {
                    PlayFootStepSound();

                    _stepTimer = _sprintingInterval;
                }
            }
        }
    }

    private void PlayFootStepSound()
    {
        if (_playerSO.FootstepClips.Length > 0)
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f))
            {
                foreach (var item in _playerSO.FootstepClips)
                {
                    if (item.Tag == hit.transform.gameObject.tag)
                    {
                        int index = Random.Range(0, item.FootstepClips.Length);
                        SFXManager.Instance.PlaySFX(item.FootstepClips[index], _volume);
                    }
                }
            }
        }
    }

    private void PlayLandingSound()
    {
        foreach (var item in _playerSO.FootstepClips)
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f))
            {
                if (item.Tag == hit.transform.gameObject.tag)
                {
                    SFXManager.Instance.PlaySFX(item.LandingClip, _volume);
                }
            }
        }
    }
}