using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnemyNavigationSystem : ISystem
{
    private const float DistanceToleranceSq = 0.01f;
    private const float RotationSmoothness = 1f;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (enemyNavigation, moving, transform) in
            SystemAPI.Query<RefRW<EnemyNavigation>, RefRW<Moving>, RefRW<LocalTransform>>())
        {
            RefRO<LocalToWorld> faceLtw = SystemAPI.GetComponentRO<LocalToWorld>(moving.ValueRO.FaceEntity);
            moving.ValueRW.LookingDelta = GetRotation(transform, faceLtw, enemyNavigation.ValueRO.CurrentTarget);

            float3 directionRaw = enemyNavigation.ValueRO.CurrentTarget - transform.ValueRO.Position;
            float distanceToTarget = math.lengthsq(directionRaw);

            if (distanceToTarget > DistanceToleranceSq)
            {
                moving.ValueRW.Direction = new float2(0f, 1f);
            }
            else
            {
                moving.ValueRW.Direction = default;
            }
        }
    }

    [BurstCompile]
    private float2 GetRotation(RefRW<LocalTransform> bodyLt, RefRO<LocalToWorld> faceLtw, float3 currentTarget)
    {
        float3 direction = currentTarget - faceLtw.ValueRO.Position;

        // YAW
        float3 flatDirection = direction;
        flatDirection.y = 0f;
        flatDirection = math.normalize(flatDirection);

        float3 bodyForward = bodyLt.ValueRO.Forward();

        float yawDelta = math.atan2(
            math.cross(bodyForward, flatDirection).y,
            math.dot(bodyForward, flatDirection)
        );

        // PITCH
        float distanceXZ = math.length(new float2(direction.x, direction.z));
        float pitchTarget = math.atan2(direction.y, distanceXZ);

        float3 faceForward = faceLtw.ValueRO.Forward;
        float currentPitch = math.atan2(
            faceForward.y,
            math.length(new float2(faceForward.x, faceForward.z))
        );

        float pitchDelta = pitchTarget - currentPitch;

        return new float2(math.degrees(yawDelta), math.degrees(pitchDelta)) / RotationSmoothness;
    }
}