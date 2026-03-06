using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct CirclePatrollingSystem : ISystem
{
    private const int PatrolCirclePoints = 10;
    private const float DistanceToleranceSq = 0.5f;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (patrolling, navigation, ltw) in
            SystemAPI.Query<RefRO<Patrolling>, RefRW<Navigation>, RefRO<LocalToWorld>>())
        {
            if (patrolling.ValueRO.PatrolCircleEntity != Entity.Null)
            {
                NativeList<float3> positions = GetPositions(ref state, patrolling.ValueRO.PatrolCircleEntity);

                int targetIndex = positions.IndexOf(navigation.ValueRO.Target);

                if (math.distancesq(ltw.ValueRO.Position, navigation.ValueRO.Target) < DistanceToleranceSq)
                    navigation.ValueRW.Target = positions[(targetIndex + 1) % positions.Length];

                positions.Dispose();
            }
        }
    }

    [BurstCompile]
    private NativeList<float3> GetPositions(ref SystemState state, Entity patrolCircleEntity)
    {
        RefRO<LocalToWorld> patrolCircleLtw = SystemAPI.GetComponentRO<LocalToWorld>(patrolCircleEntity);
        RefRO<PatrolCircle> patrolCircle = SystemAPI.GetComponentRO<PatrolCircle>(patrolCircleEntity);

        NativeList<float3> positions = new NativeList<float3>(PatrolCirclePoints, Allocator.Temp);

        float circlePart = math.PI2 / PatrolCirclePoints;

        for (int i = 0; i < PatrolCirclePoints; i++)
        {
            positions.Add(
                patrolCircle.ValueRO.Radius *
                new float3(math.cos(i * circlePart), 0f, math.sin(i * circlePart)) +
                patrolCircleLtw.ValueRO.Position);
        }

        return positions;
    }
}
