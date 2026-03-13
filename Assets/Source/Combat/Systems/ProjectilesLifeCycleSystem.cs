using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

partial struct ProjectilesLifeCycleSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (projectile, entity) in SystemAPI.Query<RefRW<Projectile>>().WithEntityAccess())
        {
            projectile.ValueRW.LifeSpan -= Time.deltaTime;

            if (projectile.ValueRO.LifeSpan <= 0f)
                buffer.DestroyEntity(entity);
        }

        buffer.Playback(state.EntityManager);
        buffer.Dispose();
    }
}
