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
            Move(ref state, transform, moving, velocity, entity);
            Rotate(ref state, transform, moving);
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

    [BurstCompile]
    private void Move(ref SystemState state, RefRW<LocalTransform> transform, RefRW<Moving> moving,
        RefRW<PhysicsVelocity> velocity, Entity entity)
    {
        float3 verticalVelocity = new float3(0f, velocity.ValueRO.Linear.y, 0f);
        float speed = moving.ValueRO.Speed * (moving.ValueRO.Sprinting ? 2f : 1f);

        float3 inputDirection =
            transform.ValueRO.Right() * moving.ValueRO.Direction.x +
            transform.ValueRO.Forward() * moving.ValueRO.Direction.y;

        float3 acceleration = CalculateAcceleration(ref state, inputDirection, moving, velocity, entity, speed);

        velocity.ValueRW.Linear += SystemAPI.Time.DeltaTime * moving.ValueRO.Acceleration * acceleration;
        velocity.ValueRW.Linear = ClampVelocity(velocity.ValueRO.Linear, verticalVelocity, speed);
    }

    [BurstCompile]
    private float3 CalculateAcceleration(ref SystemState state, float3 inputDirection, RefRW<Moving> moving,
        RefRW<PhysicsVelocity> velocity, Entity entity, float currentSpeed)
    {
        moving.ValueRW.ControlImpactPortion = math.min(1f, moving.ValueRO.ControlImpactPortion + SystemAPI.Time.DeltaTime);

        inputDirection *= currentSpeed * moving.ValueRO.ControlImpactPortion;

        if (moving.ValueRO.IsGrounded == false && SystemAPI.HasComponent<Jumper>(entity))
            inputDirection *= 0.2f;
        else if (math.all(inputDirection == float3.zero))
            inputDirection = -new float3(velocity.ValueRO.Linear.x, 0f, velocity.ValueRO.Linear.z);

        return inputDirection;
    }

    [BurstCompile]
    private float3 ClampVelocity(float3 velocity, float3 verticalVelocity, float speed)
    {
        float currentVelocityMagnitude = math.lengthsq(new float3(velocity.x, 0f, velocity.z));

        if (currentVelocityMagnitude > math.square(speed))
            velocity = math.normalize(new float3(velocity.x, 0f, velocity.z)) * speed + verticalVelocity;

        return velocity;
    }

    [BurstCompile]
    private void Rotate(ref SystemState state, RefRW<LocalTransform> transform, RefRW<Moving> moving)
    {
        float2 lookingDelta = moving.ValueRO.LookingDelta * SystemAPI.Time.DeltaTime;

        transform.ValueRW.Rotation = transform.ValueRW.RotateY(lookingDelta.x).Rotation;
        moving.ValueRW.VerticalRotation = math.clamp(
                moving.ValueRO.VerticalRotation - lookingDelta.y, -math.PIHALF, math.PIHALF);

        var faceTransform = SystemAPI.GetComponentRW<LocalTransform>(moving.ValueRO.FaceEntity);
        faceTransform.ValueRW.Rotation = quaternion.EulerXYZ(new float3(moving.ValueRO.VerticalRotation, 0f, 0f));
    }
}
