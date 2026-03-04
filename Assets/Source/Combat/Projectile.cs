using Unity.Entities;

public struct Projectile : IComponentData
{
    public float LifeSpan;
    public float Damage;
}
