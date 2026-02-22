using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

partial struct UnitMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, moving) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Moving>>())
        {
            float3 direction =
                transform.ValueRO.Right() * moving.ValueRO.Direction.x +
                transform.ValueRO.Forward() * moving.ValueRO.Direction.y;

            direction *= SystemAPI.Time.DeltaTime * moving.ValueRO.DefaultSpeed * (moving.ValueRO.Sprinting ? 2f : 1f);
            float2 lookingDelta = moving.ValueRO.LookingDelta * SystemAPI.Time.DeltaTime;

            transform.ValueRW.Position += direction;
            transform.ValueRW.Rotation = transform.ValueRW.RotateY(lookingDelta.x).Rotation;
            moving.ValueRW.VerticalRotation = math.clamp(moving.ValueRO.VerticalRotation - lookingDelta.y, -math.PIHALF, math.PIHALF);

            var faceTransform = SystemAPI.GetComponentRW<LocalTransform>(moving.ValueRO.FaceEntity);
            faceTransform.ValueRW.Rotation = quaternion.EulerXYZ(new float3(moving.ValueRO.VerticalRotation, 0f, 0f));
        }

        foreach (var (physicsVelocity, jumper) in SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<Jumper>>())
        {
            if (jumper.ValueRO.IsGrounded && jumper.ValueRO.ReadyToJump)
            {
                float3 linier = physicsVelocity.ValueRO.Linear;
                physicsVelocity.ValueRW.Linear = new float3(linier.x, jumper.ValueRO.JumpForce, linier.z);
                jumper.ValueRW.ReadyToJump = false;
            }
        }
    }
}
