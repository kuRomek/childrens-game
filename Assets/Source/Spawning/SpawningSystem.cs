using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SpawningSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (spawner, ltw) in SystemAPI.Query<RefRW<Spawner>, RefRO<LocalToWorld>>())
        {
            if (spawner.ValueRO.CountLeft == 0)
                continue;

            spawner.ValueRW.AccumSeconds += SystemAPI.Time.DeltaTime;

            if (spawner.ValueRO.AccumSeconds >= spawner.ValueRO.Rate)
            {
                Entity unit = buffer.Instantiate(spawner.ValueRO.EnemyPrefabEntity);
                buffer.SetComponent(unit, new Patrolling() { PatrolCircleEntity = spawner.ValueRO.PatrolCircle, });

                spawner.ValueRW.CountLeft--;
                spawner.ValueRW.AccumSeconds = 0f;
            }
        }

        buffer.Playback(state.EntityManager);
        buffer.Dispose();
    }
}
