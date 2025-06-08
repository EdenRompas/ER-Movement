using UnityEngine;

public class PlayerFootStepSound : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _footstepClips;

    [SerializeField] private float _walkingInterval = 0.5f;
    [SerializeField] private float _sprintingInterval = 0.3f;

    private float _stepTimer;

    private void Update()
    {
        if ((_playerMovement.IsWalking || _playerMovement.IsSprinting) && _characterController.isGrounded)
        {
            _stepTimer -= Time.deltaTime;

            if (_stepTimer <= 0f)
            {
                if (_playerMovement.IsWalking)
                {
                    PlayFootstep();

                    _stepTimer = _walkingInterval;
                }
                else if (_playerMovement.IsSprinting)
                {
                    PlayFootstep();

                    _stepTimer = _sprintingInterval;
                }
            }
        }
        else
        {
            _stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (_footstepClips.Length > 0)
        {
            int index = Random.Range(0, _footstepClips.Length);
            _audioSource.PlayOneShot(_footstepClips[index]);
        }
    }
}