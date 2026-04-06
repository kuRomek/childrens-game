using Unity.Entities;
using Unity.Mathematics;

public struct Moving : IComponentData
{
    public Entity FaceEntity;
    public float2 Direction;
    public float2 LookingDelta;
    public float VerticalRotation;
    public float Speed;
    public float Acceleration;
    public float ControlImpactPortion;
    public bool Sprinting;
    public bool IsGrounded;
}