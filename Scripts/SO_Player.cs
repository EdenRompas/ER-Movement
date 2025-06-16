using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Player/Stats")]
public class SO_Player : ScriptableObject
{
    public float WalkingSpeed;
    public float RunningSpeed;
    public float JumpHeight;
}