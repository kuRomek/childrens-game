using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct HealthSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (damage, entity) in SystemAPI.Query<RefRO<Damage>>().WithEntityAccess())
        {
            if (SystemAPI.HasComponent<Health>(damage.ValueRO.SubjectEntity))
            {
                var health = SystemAPI.GetComponentRW<Health>(damage.ValueRO.SubjectEntity);
                health.ValueRW.Current = math.clamp(health.ValueRO.Current - damage.ValueRO.Amount, 0f, health.ValueRO.Max);

                if (health.ValueRO.BarEntity != Entity.Null)
                    UpdateBar(ref state, (RefRO<Health>)health, ref entityCommandBuffer);
            }

            entityCommandBuffer.DestroyEntity(entity);
        }

        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    private void UpdateBar(ref SystemState state, RefRO<Health> health, ref EntityCommandBuffer entityCommandBuffer)
    {
        if (SystemAPI.HasComponent<PostTransformMatrix>(health.ValueRO.BarEntity))
        {
            SystemAPI.GetComponentRW<PostTransformMatrix>(health.ValueRO.BarEntity).ValueRW.Value =
                float4x4.Scale(health.ValueRO.Current / health.ValueRO.Max, 0.1f, 1f);
        }
        else
        {
            entityCommandBuffer.AddComponent(health.ValueRO.BarEntity, new PostTransformMatrix()
            {
                Value = float4x4.Scale(health.ValueRO.Current / health.ValueRO.Max, 0.1f, 1f),
            });
        }
    }
}
