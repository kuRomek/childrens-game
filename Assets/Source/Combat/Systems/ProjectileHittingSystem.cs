using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
partial struct ProjectileHittingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer buffer = SystemAPI.
            GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>().
            CreateCommandBuffer(state.WorldUnmanaged);

        ProjectileHittingEvent hittingEvent = new ProjectileHittingEvent()
        {
            Buffer = buffer,
            LookupHealth = SystemAPI.GetComponentLookup<Health>(),
            LookupProjectile = SystemAPI.GetComponentLookup<Projectile>(),
            LookupCollider = SystemAPI.GetComponentLookup<PhysicsCollider>(),
            LookupVelocity = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
            LookupLtw = SystemAPI.GetComponentLookup<LocalToWorld>(),
        };

        state.Dependency = hittingEvent.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    public struct ProjectileHittingEvent : ITriggerEventsJob
    {
        public EntityCommandBuffer Buffer;

        [ReadOnly] public ComponentLookup<Projectile> LookupProjectile;
        [ReadOnly] public ComponentLookup<Health> LookupHealth;
        [ReadOnly] public ComponentLookup<PhysicsCollider> LookupCollider;
        [ReadOnly] public ComponentLookup<PhysicsVelocity> LookupVelocity;
        [ReadOnly] public ComponentLookup<LocalToWorld> LookupLtw;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity damageTakerEntity = Entity.Null;
            Entity projectileEntity = Entity.Null;

            RefRO<Projectile> projectile = default;

            ExtractEntities(ref triggerEvent, ref damageTakerEntity, ref projectileEntity);

            if (projectileEntity != null)
                LookupProjectile.TryGetRefRO(projectileEntity, out projectile);

            if (projectileEntity != Entity.Null && TryDestroyProjectile(triggerEvent, projectileEntity))
            {
                Buffer.AddComponent(Buffer.CreateEntity(), new SoundEffect()
                {
                    SoundKey = AudioKeys.Sound.LaserGunHit,
                    PitchRange = new(0.9f, 1.1f),
                    Position = LookupLtw.GetRefRO(projectileEntity).ValueRO.Position,
                });
            }

            if (damageTakerEntity == Entity.Null || projectileEntity == Entity.Null)
                return;

            LookupVelocity.TryGetComponent(projectileEntity, out PhysicsVelocity velocity);

            Buffer.AddComponent(Buffer.CreateEntity(), new Damage()
            {
                Amount = projectile.ValueRO.Damage,
                DamageTakerEntity = damageTakerEntity,
                Force = projectile.ValueRO.DamageForce,
                ForceDirection = math.normalize(velocity.Linear) + new float3(0f, 0.5f, 0f),
                DamageDealerEntity = projectile.ValueRO.ShooterEntity,
            });
        }

        private void ExtractEntities(ref TriggerEvent triggerEvent, ref Entity damageTakerEntity, ref Entity projectileEntity)
        {
            if (LookupProjectile.HasComponent(triggerEvent.EntityA))
                projectileEntity = triggerEvent.EntityA;
            else if (LookupProjectile.HasComponent(triggerEvent.EntityB))
                projectileEntity = triggerEvent.EntityB;

            if (LookupHealth.HasComponent(triggerEvent.EntityA))
                damageTakerEntity = triggerEvent.EntityA;
            else if (LookupHealth.HasComponent(triggerEvent.EntityB))
                damageTakerEntity = triggerEvent.EntityB;
        }

        private bool TryDestroyProjectile(TriggerEvent triggerEvent, Entity projectileEntity)
        {
            if (projectileEntity == triggerEvent.EntityA &&
                HasCollider(triggerEvent.EntityB, triggerEvent.ColliderKeyB, LookupCollider))
            {
                Buffer.DestroyEntity(triggerEvent.EntityA);
                return true;
            }
            else if (projectileEntity == triggerEvent.EntityB &&
                HasCollider(triggerEvent.EntityA, triggerEvent.ColliderKeyA, LookupCollider))
            {
                Buffer.DestroyEntity(triggerEvent.EntityB);
                return true;
            }

            return false;
        }

        private bool HasCollider(Entity entity, ColliderKey colliderKey, ComponentLookup<PhysicsCollider> colliderLookup)
        {
            if (colliderLookup.HasComponent(entity) == false)
                return false;

            return colliderLookup[entity].Value.Value.GetCollisionResponse(colliderKey) == CollisionResponsePolicy.Collide;
        }
    }
}
