using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerMovement _playerMovement;

    private void OnEnable()
    {
        _playerMovement.OnIdle += SetIdle;
        _playerMovement.OnWalking += SetWalking;
        _playerMovement.OnSprinting += SetSprinting;
        _playerMovement.OnJumping += SetJumping;
        _playerMovement.OnJumpWhileMoving += SetJumpingWhileMove;
    }

    private void OnDisable()
    {
        _playerMovement.OnIdle -= SetIdle;
        _playerMovement.OnWalking -= SetWalking;
        _playerMovement.OnSprinting -= SetSprinting;
        _playerMovement.OnJumping -= SetJumping;
        _playerMovement.OnJumpWhileMoving -= SetJumpingWhileMove;
    }

    private void SetIdle()
    {
        _animator.SetBool("IsIdle", true);
        _animator.SetBool("IsWalking", false);
        _animator.SetBool("IsSprinting", false);
    }

    private void SetWalking()
    {
        _animator.SetBool("IsWalking", true);
        _animator.SetBool("IsIdle", false);
        _animator.SetBool("IsSprinting", false);
    }

    private void SetSprinting()
    {
        _animator.SetBool("IsSprinting", true);
        _animator.SetBool("IsWalking", false);
        _animator.SetBool("IsIdle", false);
    }

    private void SetJumping()
    {
        _animator.SetTrigger("Jump");
        _animator.SetBool("IsIdle", false);
        _animator.SetBool("IsWalking", false);
        _animator.SetBool("IsSprinting", false);
    }

    private void SetJumpingWhileMove()
    {
        _animator.SetTrigger("JumpWhileMove");
        _animator.SetBool("IsIdle", false);
        _animator.SetBool("IsWalking", false);
        _animator.SetBool("IsSprinting", false);
    }
}