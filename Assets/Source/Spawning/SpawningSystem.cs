using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SpawningSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (spawner, ltw) in SystemAPI.Query<RefRW<Spawner>, RefRO<LocalToWorld>>())
        {
            if (spawner.ValueRO.CountLeft == 0)
                continue;

            spawner.ValueRW.AccumSeconds += SystemAPI.Time.DeltaTime;

            if (spawner.ValueRO.AccumSeconds >= spawner.ValueRO.Rate)
            {
                Entity unit = state.EntityManager.Instantiate(spawner.ValueRO.EnemyPrefabEntity);
                SystemAPI.GetComponentRW<Patrolling>(unit).ValueRW.PatrolCircleEntity = spawner.ValueRO.PatrolCircle;
                SystemAPI.GetComponentRW<LocalTransform>(unit).ValueRW.Position = ltw.ValueRO.Position;

                spawner.ValueRW.CountLeft--;
                spawner.ValueRW.AccumSeconds = 0f;
            }
        }
    }
}
