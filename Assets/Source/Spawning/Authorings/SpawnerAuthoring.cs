using Unity.Entities;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    [field: SerializeField] public EnemyAuthoring EnemyPrefab { get; private set; }
    [field: SerializeField] public PatrolCircleAuthoring PatrolCircle { get; private set; }
}

public class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.WorldSpace);

        AddComponent(entity, new Spawner()
        {
            Rate = Configs.Spawning.Rate,
            CountLeft = Configs.Spawning.Count,
            AccumSeconds = 0f,
            PatrolCircle = GetEntity(authoring.PatrolCircle, TransformUsageFlags.WorldSpace),
            EnemyPrefabEntity = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
        });
    }
}
