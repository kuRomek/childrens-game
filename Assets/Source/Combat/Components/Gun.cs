using Unity.Entities;

public struct Gun : IComponentData
{
    public Entity ProjectilePrefab;
    public float Rate;
    public float Cooldown;
    public float Damage;
}
