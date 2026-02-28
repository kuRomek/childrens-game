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

        ApplyDamage(ref state, ref entityCommandBuffer);
        UpdateHealthBarRotation(ref state);

        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    private void ApplyDamage(ref SystemState state, ref EntityCommandBuffer entityCommandBuffer)
    {
        foreach (var (damage, entity) in SystemAPI.Query<RefRO<Damage>>().WithEntityAccess())
        {
            if (SystemAPI.HasComponent<Health>(damage.ValueRO.SubjectEntity))
            {
                var health = SystemAPI.GetComponentRW<Health>(damage.ValueRO.SubjectEntity);
                health.ValueRW.Current = math.clamp(health.ValueRO.Current - damage.ValueRO.Amount, 0f, health.ValueRO.Max);

                if (health.ValueRO.BarEntity != Entity.Null)
                {
                    UpdateBar(ref state, health.ValueRO.BarEntity,
                        float4x4.Scale(health.ValueRO.Current / health.ValueRO.Max, 0.1f, 1f), ref entityCommandBuffer);
                }
            }

            entityCommandBuffer.DestroyEntity(entity);
        }
    }

    [BurstCompile]
    private void UpdateHealthBarRotation(ref SystemState state)
    {
        Entity playerEntity = SystemAPI.GetSingleton<Player>().Entity;
        RefRO<LocalToWorld> faceTransform = SystemAPI.GetComponentRO<LocalToWorld>(
            SystemAPI.GetComponentRO<Moving>(playerEntity).ValueRO.FaceEntity);

        foreach (var (health, parentLtw) in SystemAPI.Query<RefRO<Health>, RefRO<LocalToWorld>>())
        {
            Entity barEntity = health.ValueRO.BarEntity;

            if (barEntity != Entity.Null)
            {
                RefRW<LocalTransform> barLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(barEntity);
                RefRO<LocalToWorld> barLtw = SystemAPI.GetComponentRO<LocalToWorld>(barEntity);

                float3 dir = math.normalizesafe(faceTransform.ValueRO.Position - barLtw.ValueRO.Position);

                quaternion worldRot = quaternion.LookRotationSafe(dir, math.up());

                barLocalTransform.ValueRW.Rotation =
                    math.mul(math.inverse(parentLtw.ValueRO.Rotation), worldRot);
            }
        }
    }

    [BurstCompile]
    private void UpdateBar(ref SystemState state, Entity entity, float4x4 transform, ref EntityCommandBuffer entityCommandBuffer)
    {
        if (SystemAPI.HasComponent<PostTransformMatrix>(entity))
            SystemAPI.GetComponentRW<PostTransformMatrix>(entity).ValueRW.Value = transform;
        else
            entityCommandBuffer.AddComponent(entity, new PostTransformMatrix() { Value = transform });
    }
}
