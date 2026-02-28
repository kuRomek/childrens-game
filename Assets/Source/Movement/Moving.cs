using Unity.Entities;
using Unity.Mathematics;

public struct Moving : IComponentData
{
    public Entity FaceEntity;
    public float2 Direction;
    public float2 LookingDelta;
    public float VerticalRotation;
    public float DefaultSpeed;
    public bool Sprinting;
    public bool IsGrounded;
}