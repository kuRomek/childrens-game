using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

partial struct PlayerInputHandleSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
        state.RequireForUpdate<Moving>();

        state.RequireForUpdate<PlayerInput>();
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
        RefRW<Attacker> playerAttacker = SystemAPI.GetComponentRW<Attacker>(playerEntity);

        playerMoving.ValueRW.Direction = new float2(input.MovingDirection.x, input.MovingDirection.z);
        playerMoving.ValueRW.LookingDelta = lookingDelta;

        if (playerMoving.ValueRO.IsGrounded)
            playerMoving.ValueRW.Sprinting = input.Sprinting;

        playerJumper.ValueRW.ReadyToJump |= input.HasJumped && playerMoving.ValueRO.IsGrounded;
        playerAttacker.ValueRW.Attacking = input.Shooting;
    }
}
