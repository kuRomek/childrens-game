using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct HealthSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TestConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        ApplyDamage(ref state, ref buffer);
        UpdateHealthBarRotation(ref state);

        buffer.Playback(state.EntityManager);
        buffer.Dispose();
    }

    [BurstCompile]
    private void ApplyDamage(ref SystemState state, ref EntityCommandBuffer buffer)
    {
        var config = SystemAPI.GetSingletonRW<TestConfig>();

        foreach (var (damage, entity) in SystemAPI.Query<RefRO<Damage>>().WithEntityAccess())
        {
            if (SystemAPI.HasComponent<Health>(damage.ValueRO.DamageTakerEntity))
            {
                var health = SystemAPI.GetComponentRW<Health>(damage.ValueRO.DamageTakerEntity);

                if (health.ValueRO.IsDead)
                    continue;

                health.ValueRW.Current = math.clamp(health.ValueRO.Current - damage.ValueRO.Amount, 0f, health.ValueRO.Max);

                if (SystemAPI.HasComponent<Player>(damage.ValueRO.DamageTakerEntity))
                {
                    buffer.AddComponent(buffer.CreateEntity(), new PlayerHealthViewUpdate()
                    {
                        CurrentValue = health.ValueRO.Current,
                        MaxValue = health.ValueRO.Max,
                    });
                }

                if (health.ValueRO.IsDead)
                {
                    if (SystemAPI.HasComponent<Player>(damage.ValueRO.DamageTakerEntity) &&
                        SystemAPI.HasSingleton<RunEnd>() == false)
                    {
                        buffer.AddComponent(buffer.CreateEntity(), new RunEnd() { Delay = 3f });
                    }

                    if (SystemAPI.HasComponent<Player>(damage.ValueRO.DamageDealerEntity))
                        buffer.AddComponent(buffer.CreateEntity(), new EngagementBurst() { Amount = config.ValueRO.EngagementBurstForKilledEnemy });

                    buffer.DestroyEntity(damage.ValueRO.DamageTakerEntity);
                }

                ApplyForce(ref state, damage);

                UpdateBar(ref state, health.ValueRO.BarEntity,
                    float4x4.Scale(health.ValueRO.Current / health.ValueRO.Max, 0.1f, 1f), ref buffer);
            }

            buffer.DestroyEntity(entity);
        }
    }

    [BurstCompile]
    private void ApplyForce(ref SystemState state, RefRO<Damage> damage)
    {
        if (SystemAPI.HasComponent<PhysicsVelocity>(damage.ValueRO.DamageTakerEntity) &&
            SystemAPI.HasComponent<PhysicsMass>(damage.ValueRO.DamageTakerEntity))
        {
            var velocity = SystemAPI.GetComponentRW<PhysicsVelocity>(damage.ValueRO.DamageTakerEntity);
            var mass = SystemAPI.GetComponentRW<PhysicsMass>(damage.ValueRO.DamageTakerEntity);

            velocity.ValueRW.Linear += mass.ValueRO.InverseMass * damage.ValueRO.Force *
                damage.ValueRO.ForceDirection;

            if (SystemAPI.HasComponent<Moving>(damage.ValueRO.DamageTakerEntity))
                SystemAPI.GetComponentRW<Moving>(damage.ValueRO.DamageTakerEntity).ValueRW.ControlImpactPortion = 0f;
        }
    }

    [BurstCompile]
    private void UpdateHealthBarRotation(ref SystemState state)
    {
        if (SystemAPI.TryGetSingleton(out Player player))
        {
            Entity playerEntity = player.Entity;
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
    }

    [BurstCompile]
    private void UpdateBar(ref SystemState state, Entity healthBarEntity, float4x4 transform,
        ref EntityCommandBuffer entityCommandBuffer)
    {
        if (healthBarEntity != Entity.Null)
        {
            if (SystemAPI.HasComponent<PostTransformMatrix>(healthBarEntity))
                SystemAPI.GetComponentRW<PostTransformMatrix>(healthBarEntity).ValueRW.Value = transform;
            else
                entityCommandBuffer.AddComponent(healthBarEntity, new PostTransformMatrix() { Value = transform });
        }
    }
}
