using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.GraphicsIntegration;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
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
                var unitLt = SystemAPI.GetComponentRW<LocalTransform>(unit);
                unitLt.ValueRW.Position = ltw.ValueRO.Position;

                if (SystemAPI.HasComponent<PhysicsGraphicalInterpolationBuffer>(unit))
                {
                    var rigidTransform = new RigidTransform(unitLt.ValueRO.ToMatrix());

                    SystemAPI.SetComponent(unit, new PhysicsGraphicalInterpolationBuffer()
                    {
                        PreviousTransform = rigidTransform,
                        PreviousVelocity = SystemAPI.GetComponentRO<PhysicsVelocity>(unit).ValueRO,
                    });
                }

                spawner.ValueRW.CountLeft--;
                spawner.ValueRW.AccumSeconds = 0f;
            }
        }
    }
}
