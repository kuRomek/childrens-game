using Unity.Entities;
using Unity.Mathematics;

public struct Jumper : IComponentData
{
    public float3 VelocityAtJump;
    public bool ReadyToJump;
    public float JumpForce;
}
