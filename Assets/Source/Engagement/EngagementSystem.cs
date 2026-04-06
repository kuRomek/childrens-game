using Unity.Entities;
using Unity.Mathematics;

partial struct EngagementSystem : ISystem
{
    private Entity _engagementEntity;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Engagement>();
        state.RequireForUpdate<Player>();
        state.RequireForUpdate<TestConfig>();
        state.RequireForUpdate<GameState>();

        _engagementEntity = state.EntityManager.CreateSingleton<Engagement>();
        state.EntityManager.SetComponentData(_engagementEntity, new Engagement()
        {
            Current = 100f,
            Max = 100f,
        });
    }

    public void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.GetSingleton<GameState>().IsInRun == false)
            return;

        var engagement = SystemAPI.GetSingletonRW<Engagement>();
        var config = SystemAPI.GetSingletonRW<TestConfig>();

        if (engagement.ValueRO.Current == 0f)
            return;

        var buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        float engagementDelta = -SystemAPI.Time.DeltaTime * config.ValueRO.EngagementFadeSpeed;

        foreach (var (engagementBurst, entity) in SystemAPI.Query<RefRO<EngagementBurst>>().WithEntityAccess())
        {
            engagementDelta += engagementBurst.ValueRO.Amount;
            buffer.DestroyEntity(entity);
        }

        if (engagementDelta != 0f)
        {
            engagement.ValueRW.Current = math.clamp(engagement.ValueRO.Current + engagementDelta, 0f, engagement.ValueRW.Max);
            EngagementView.UpdateBar(engagement.ValueRO.Current, engagement.ValueRO.Max);
        }

        if (engagement.ValueRO.Current == 0f && SystemAPI.HasSingleton<RunEnd>() == false)
        {
            engagement.ValueRW.Current = engagement.ValueRO.Max;
            buffer.AddComponent(buffer.CreateEntity(), new RunEnd() { Delay = 3f });
        }

        buffer.Playback(state.EntityManager);
        buffer.Dispose();
    }

    public void OnDestroy(ref SystemState state)
    {
        state.EntityManager.DestroyEntity(_engagementEntity);
    }
}
