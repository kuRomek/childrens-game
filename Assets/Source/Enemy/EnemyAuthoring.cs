using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(UnitAuthoring))]
public class EnemyAuthoring : MonoBehaviour { }

public class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Enemy() { Entity = entity });
        AddComponent(entity, new EnemyNavigation() { CurrentTarget = default });
    }
}
