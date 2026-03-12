using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
partial struct GroundDetectionSystem : ISystem
{
    private NativeParallelHashSet<GroundInteractionPair> _previousFrame;
    private NativeParallelHashSet<GroundInteractionPair> _currentFrame;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _previousFrame = new(128, Allocator.Persistent);
        _currentFrame = new(128, Allocator.Persistent);

        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _currentFrame.Clear();

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        var groundTouching = new GroundTouchingEvent()
        {
            CurrentFrame = _currentFrame.AsParallelWriter(),
            LookupGround = SystemAPI.GetComponentLookup<Ground>(true),
            LookupJumper = SystemAPI.GetComponentLookup<Jumper>(true),
        };

        state.Dependency = groundTouching.Schedule(simulation, state.Dependency);
        state.Dependency.Complete();

        foreach (GroundInteractionPair groundInteractionPair in _currentFrame)
            SystemAPI.GetComponentRW<Moving>(groundInteractionPair.JumperEntity).ValueRW.IsGrounded = true;

        foreach (GroundInteractionPair groundInteractionPair in _previousFrame)
        {
            SystemAPI.GetComponentRW<Moving>(groundInteractionPair.JumperEntity).ValueRW.IsGrounded =
                _currentFrame.Contains(groundInteractionPair);
        }

        (_previousFrame, _currentFrame) = (_currentFrame, _previousFrame);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (_previousFrame.IsCreated)
            _previousFrame.Dispose();

        if (_currentFrame.IsCreated)
            _currentFrame.Dispose();
    }

    [BurstCompile]
    public partial struct GroundTouchingEvent : ITriggerEventsJob
    {
        public NativeParallelHashSet<GroundInteractionPair>.ParallelWriter CurrentFrame;

        [ReadOnly] public ComponentLookup<Ground> LookupGround;
        [ReadOnly] public ComponentLookup<Jumper> LookupJumper;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity jumperEntity = Entity.Null;
            Entity groundEntity = Entity.Null;

            if (LookupGround.HasComponent(triggerEvent.EntityA) && LookupJumper.HasComponent(triggerEvent.EntityB))
            {
                groundEntity = triggerEvent.EntityA;
                jumperEntity = triggerEvent.EntityB;
            }
            else if (LookupJumper.HasComponent(triggerEvent.EntityA) && LookupGround.HasComponent(triggerEvent.EntityB))
            {
                groundEntity = triggerEvent.EntityB;
                jumperEntity = triggerEvent.EntityA;
            }

            if (jumperEntity != Entity.Null && groundEntity != Entity.Null)
                CurrentFrame.Add(new GroundInteractionPair() { JumperEntity = jumperEntity, GroundEntity = groundEntity });
        }
    }
}
