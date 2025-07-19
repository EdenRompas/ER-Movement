using System;
using UnityEngine;

[Serializable]
public class FootstepClip
{
    public string Tag;
    public AudioClip[] FootstepClips;
    public AudioClip LandingClip;
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/New Stats")]
public class SO_Player : ScriptableObject
{
    public float WalkingSpeed;
    public float RunningSpeed;
    public float JumpHeight;

    public FootstepClip[] FootstepClips;
}