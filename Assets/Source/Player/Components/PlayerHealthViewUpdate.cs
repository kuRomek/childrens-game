using Unity.Entities;

public struct PlayerHealthViewUpdate : IComponentData
{
    public float CurrentValue;
    public float MaxValue;
}