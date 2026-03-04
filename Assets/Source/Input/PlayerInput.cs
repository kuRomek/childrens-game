using Unity.Entities;
using Unity.Mathematics;

public struct PlayerInput : IComponentData
{
    public float3 MovingDirection;
    public float2 LookingDelta;
    public bool HasJumped;
    public bool Sprinting;
    public bool Shooting;
}
