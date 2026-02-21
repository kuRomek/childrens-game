using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct PlayerMovingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerInput input = SystemAPI.GetSingleton<PlayerInput>();
        Entity playerEntity = SystemAPI.GetSingleton<Player>().Entity;
        PlayerConfig playerConfig = SystemAPI.GetSingleton<PlayerConfig>();
        float lookingDelta = playerConfig.MouseSensitivity * SystemAPI.Time.DeltaTime * input.LookingDelta.x;

        RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(playerEntity);

        localTransform.ValueRW.Rotation = localTransform.ValueRW.RotateY(lookingDelta).Rotation;

        float3 direction =
            localTransform.ValueRO.Right() * input.MovingDirection.x +
            localTransform.ValueRO.Forward() * input.MovingDirection.z;

        localTransform.ValueRW.Position += playerConfig.DefaultSpeed * SystemAPI.Time.DeltaTime * direction;

        RefRW<Jumper> jumper = SystemAPI.GetComponentRW<Jumper>(playerEntity);
        RefRW<PhysicsVelocity> physicsVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(playerEntity);

        physicsVelocity.ValueRW.Angular = new float3(0f, physicsVelocity.ValueRO.Angular.y, 0f);

        if (input.HasJumped && jumper.ValueRO.IsGrounded)
            physicsVelocity.ValueRW.Linear += new float3(0f, playerConfig.JumpForce, 0f);
    }
}
