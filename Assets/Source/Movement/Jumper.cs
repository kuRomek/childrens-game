using Unity.Entities;

public struct Jumper : IComponentData
{
    public bool IsGrounded;
    public bool ReadyToJump;
    public float JumpForce;
}
