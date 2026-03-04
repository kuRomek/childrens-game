using Unity.Entities;

public struct Attacker : IComponentData
{
    public bool Attacking;
    public Entity WeaponEntity;
}
