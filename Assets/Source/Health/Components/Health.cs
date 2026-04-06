using Unity.Entities;

public struct Health : IComponentData
{
    public float Max;
    public float Current;
    public Entity BarEntity;

    public readonly bool IsDead => Current == 0;
}