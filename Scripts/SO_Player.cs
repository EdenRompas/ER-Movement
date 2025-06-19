using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/New Stats")]
public class SO_Player : ScriptableObject
{
    public float WalkingSpeed;
    public float RunningSpeed;
    public float JumpHeight;

    public AudioClip[] FootstepClips;
    public AudioClip LandingClip;
}