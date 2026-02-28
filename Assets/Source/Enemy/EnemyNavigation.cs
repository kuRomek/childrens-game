using Unity.Entities;
using Unity.Mathematics;

public struct EnemyNavigation : IComponentData
{
    public float3 CurrentTarget;
}