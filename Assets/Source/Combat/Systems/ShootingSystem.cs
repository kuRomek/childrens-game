using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.GraphicsIntegration;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(TransformSystemGroup))]
partial struct ShootingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (attacker, attackerEntity) in SystemAPI.Query<RefRW<Attacker>>().WithEntityAccess())
        {
            if (SystemAPI.HasComponent<Gun>(attacker.ValueRO.WeaponEntity) == false)
                continue;

            RefRW<Gun> gun = SystemAPI.GetComponentRW<Gun>(attacker.ValueRO.WeaponEntity);

            if (attacker.ValueRO.Attacking && gun.ValueRO.Cooldown <= 0f)
            {
                Entity projectileEntity = state.EntityManager.Instantiate(gun.ValueRO.ProjectilePrefab);
                buffer.SetEnabled(projectileEntity, true);
                RefRO<LocalToWorld> gunLtw = SystemAPI.GetComponentRO<LocalToWorld>(attacker.ValueRO.WeaponEntity);

                float3 direction = math.mul(gunLtw.ValueRO.Rotation, new float3(0f, 0f, 1f));

                RefRW<LocalTransform> projectileLt = SystemAPI.GetComponentRW<LocalTransform>(projectileEntity);
                projectileLt.ValueRW.Position = gunLtw.ValueRO.Position + direction * 0.3f;
                projectileLt.ValueRW.Rotation = gunLtw.ValueRO.Rotation;

                RefRW<PhysicsVelocity> projectileVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(projectileEntity);
                projectileVelocity.ValueRW.Linear = direction * 100f;

                RefRW<Projectile> projectile = SystemAPI.GetComponentRW<Projectile>(projectileEntity);
                projectile.ValueRW.Damage = gun.ValueRO.Damage;
                projectile.ValueRW.ShooterEntity = attackerEntity;

                RefRW<PhysicsCollider> collider = SystemAPI.GetComponentRW<PhysicsCollider>(projectileEntity);
                CollisionFilter collisionFilter = collider.ValueRO.Value.Value.GetCollisionFilter();
                collisionFilter.CollidesWith =
                    SystemAPI.HasComponent<Player>(attackerEntity) ?
                    Utils.PhysicsLayers.Enemy :
                    Utils.PhysicsLayers.Player;

                collider.ValueRW.Value.Value.SetCollisionFilter(collisionFilter);

                if (SystemAPI.HasComponent<PhysicsGraphicalInterpolationBuffer>(projectileEntity))
                {
                    var rigidTransform = new RigidTransform(projectileLt.ValueRO.ToMatrix());

                    SystemAPI.SetComponent(projectileEntity, new PhysicsGraphicalInterpolationBuffer()
                    {
                        PreviousTransform = rigidTransform,
                        PreviousVelocity = projectileVelocity.ValueRO,
                    });
                }

                Entity soundEffectEntity = buffer.CreateEntity();
                buffer.AddComponent(soundEffectEntity, new SoundEffect()
                {
                    SoundKey = AudioKeys.Sound.LaserGunShoot,
                    PitchRange = new(0.5f, 1.5f),
                    Position = gunLtw.ValueRO.Position,
                });

                gun.ValueRW.Cooldown = 1f / gun.ValueRO.Rate;
            }

            gun.ValueRW.Cooldown = math.max(gun.ValueRO.Cooldown - Time.deltaTime, 0f);
        }

        buffer.Playback(state.EntityManager);
        buffer.Dispose();
    }
}
