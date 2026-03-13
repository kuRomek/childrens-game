using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct TouchHittingSystem : ISystem
{
    private NativeParallelHashSet<EntityOrderedPair> _previousFrame;
    private NativeParallelHashSet<EntityOrderedPair> _currentFrame;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _previousFrame = new(128, Allocator.Persistent);
        _currentFrame = new(128, Allocator.Persistent);

        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<TestConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _currentFrame.Clear();

        EntityCommandBuffer buffer = SystemAPI.
            GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>().
            CreateCommandBuffer(state.WorldUnmanaged);

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        if (SystemAPI.TryGetSingletonRW(out RefRW<TestConfig> testConfig))
        {
            TouchHittingEvent hittingEvent = new TouchHittingEvent()
            {
                CurrentFrame = _currentFrame.AsParallelWriter(),
                LookupAttacker = SystemAPI.GetComponentLookup<Attacker>(),
                LookupEnemy = SystemAPI.GetComponentLookup<Enemy>(),
                LookupPlayer = SystemAPI.GetComponentLookup<Player>(),
                LookupHealth = SystemAPI.GetComponentLookup<Health>(),
            };

            state.Dependency = hittingEvent.Schedule(simulation, state.Dependency);
            state.Dependency.Complete();

            foreach (var pair in _currentFrame)
            {
                if (_previousFrame.Contains(pair) || pair.Entity1 == Entity.Null || pair.Entity2 == Entity.Null)
                    continue;

                var velocity = SystemAPI.GetComponentRO<PhysicsVelocity>(pair.Entity2);
                var position = SystemAPI.GetComponentRO<LocalToWorld>(pair.Entity2).ValueRO.Position;

                buffer.AddComponent(buffer.CreateEntity(), new SoundEffect()
                {
                    SoundKey = AudioKeys.Sound.LaserGunHit,
                    PitchRange = new(0.9f, 1.1f),
                    Position = position,
                });

                buffer.AddComponent(buffer.CreateEntity(), new Damage()
                {
                    Amount = testConfig.ValueRO.Damage,
                    SubjectEntity = pair.Entity1,
                    Force = testConfig.ValueRO.DamageForce,
                    ForceDirection = math.normalize(velocity.ValueRO.Linear) + new float3(0f, 0.5f, 0f),
                });
            }
        }

        (_currentFrame, _previousFrame) = (_previousFrame, _currentFrame);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        _previousFrame.Dispose();
        _currentFrame.Dispose();
    }

    [BurstCompile]
    public struct TouchHittingEvent : ICollisionEventsJob
    {
        public NativeParallelHashSet<EntityOrderedPair>.ParallelWriter CurrentFrame;

        [ReadOnly] public ComponentLookup<Attacker> LookupAttacker;
        [ReadOnly] public ComponentLookup<Enemy> LookupEnemy;
        [ReadOnly] public ComponentLookup<Player> LookupPlayer;
        [ReadOnly] public ComponentLookup<Health> LookupHealth;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity damageTakerEntity = Entity.Null;
            Entity attackerEntity = Entity.Null;

            ExtractEntities(ref collisionEvent, ref damageTakerEntity, ref attackerEntity);

            if (damageTakerEntity != Entity.Null && attackerEntity != Entity.Null)
                CurrentFrame.Add(new EntityOrderedPair() { Entity1 = damageTakerEntity, Entity2 = attackerEntity });
        }

        private void ExtractEntities(ref CollisionEvent collisionEvent, ref Entity damageTakerEntity, ref Entity attackerEntity)
        {
            if (LookupAttacker.HasComponent(collisionEvent.EntityA) && LookupEnemy.HasComponent(collisionEvent.EntityA))
                attackerEntity = collisionEvent.EntityA;
            else if (LookupAttacker.HasComponent(collisionEvent.EntityB) && LookupEnemy.HasComponent(collisionEvent.EntityB))
                attackerEntity = collisionEvent.EntityB;

            if (LookupHealth.HasComponent(collisionEvent.EntityA) && LookupPlayer.HasComponent(collisionEvent.EntityA))
                damageTakerEntity = collisionEvent.EntityA;
            else if (LookupHealth.HasComponent(collisionEvent.EntityB) && LookupPlayer.HasComponent(collisionEvent.EntityB))
                damageTakerEntity = collisionEvent.EntityB;
        }
    }
}
