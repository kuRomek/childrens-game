using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(UnitAuthoring))]
public class EnemyAuthoring : MonoBehaviour
{
    [field: SerializeField] public PatrolCircleAuthoring PatrolCircleAuthoring { get; private set; }
}

public class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Enemy() { Entity = entity });
        AddComponent(entity, new Navigation() { Target = default });
        AddComponent(entity, new Patrolling()
        {
            PatrolCircleEntity = GetEntity(authoring.PatrolCircleAuthoring, TransformUsageFlags.WorldSpace)
        });
    }
}
