using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateAfter(typeof(PlayerInputHandleSystem))]
partial struct UnitMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, moving, velocity, entity) in
            SystemAPI.Query<RefRW<LocalTransform>, RefRW<Moving>, RefRW<PhysicsVelocity>>().WithEntityAccess())
        {
            float3 direction =
                transform.ValueRO.Right() * moving.ValueRO.Direction.x +
                transform.ValueRO.Forward() * moving.ValueRO.Direction.y;

            if (moving.ValueRO.IsGrounded == false && SystemAPI.HasComponent<Jumper>(entity))
            {
                RefRO<Jumper> jumper = SystemAPI.GetComponentRO<Jumper>(entity);

                float3 currentHorizontalVelocity =
                    math.normalizesafe(new float3(jumper.ValueRO.VelocityAtJump.x, 0f, jumper.ValueRO.VelocityAtJump.z));
                direction = currentHorizontalVelocity * 0.3f + direction * 0.7f;
            }

            direction *= moving.ValueRO.DefaultSpeed * (moving.ValueRO.Sprinting ? 2f : 1f);

            velocity.ValueRW.Linear = new float3(direction.x, velocity.ValueRO.Linear.y, direction.z);

            float2 lookingDelta = moving.ValueRO.LookingDelta * SystemAPI.Time.DeltaTime;

            transform.ValueRW.Rotation = transform.ValueRW.RotateY(lookingDelta.x).Rotation;
            moving.ValueRW.VerticalRotation = math.clamp(
                    moving.ValueRO.VerticalRotation - lookingDelta.y, -math.PIHALF, math.PIHALF);

            var faceTransform = SystemAPI.GetComponentRW<LocalTransform>(moving.ValueRO.FaceEntity);
            faceTransform.ValueRW.Rotation = quaternion.EulerXYZ(new float3(moving.ValueRO.VerticalRotation, 0f, 0f));
        }

        foreach (var (physicsVelocity, jumper, moving) in
            SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<Jumper>, RefRW<Moving>>())
        {
            if (moving.ValueRO.IsGrounded && jumper.ValueRO.ReadyToJump)
            {
                float3 linier = physicsVelocity.ValueRO.Linear;
                physicsVelocity.ValueRW.Linear = new float3(linier.x, jumper.ValueRO.JumpForce, linier.z);
                jumper.ValueRW.VelocityAtJump = physicsVelocity.ValueRO.Linear;
                jumper.ValueRW.ReadyToJump = false;
            }
        }
    }
}
