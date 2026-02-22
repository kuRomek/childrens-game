using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

partial struct PlayerMovingInputHandleSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
        state.RequireForUpdate<Moving>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity playerEntity = SystemAPI.GetSingleton<Player>().Entity;
        PlayerInput input = SystemAPI.GetSingleton<PlayerInput>();
        PlayerConfig playerConfig = SystemAPI.GetSingleton<PlayerConfig>();
        float2 lookingDelta = playerConfig.MouseSensitivity * input.LookingDelta;

        RefRW<Moving> playerMoving = SystemAPI.GetComponentRW<Moving>(playerEntity);
        RefRW<Jumper> playerJumper = SystemAPI.GetComponentRW<Jumper>(playerEntity);

        playerMoving.ValueRW.Direction = new float2(input.MovingDirection.x, input.MovingDirection.z);
        playerMoving.ValueRW.LookingDelta = lookingDelta;
        playerMoving.ValueRW.Sprinting = input.Sprinting;

        playerJumper.ValueRW.ReadyToJump |= input.HasJumped && playerJumper.ValueRO.IsGrounded;
    }
}
