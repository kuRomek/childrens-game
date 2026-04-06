using Unity.Entities;
using Unity.Mathematics;

public struct Damage : IComponentData
{
    public Entity DamageTakerEntity;
    public Entity DamageDealerEntity;
    public float Amount;
    public float Force;
    public float3 ForceDirection;
}
