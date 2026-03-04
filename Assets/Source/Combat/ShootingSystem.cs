using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct ShootingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var attacker in SystemAPI.Query<RefRW<Attacker>>())
        {
            if (SystemAPI.HasComponent<Gun>(attacker.ValueRO.WeaponEntity) == false)
                return;

            RefRW<Gun> gun = SystemAPI.GetComponentRW<Gun>(attacker.ValueRO.WeaponEntity);

            if (attacker.ValueRO.Attacking && gun.ValueRO.Cooldown <= 0f)
            {
                Entity projectileEntity = state.EntityManager.Instantiate(gun.ValueRO.ProjectilePrefab);
                RefRO<LocalToWorld> gunLtw = SystemAPI.GetComponentRO<LocalToWorld>(attacker.ValueRO.WeaponEntity);

                float3 direction = math.mul(gunLtw.ValueRO.Rotation, new float3(0f, 0f, 1f));

                RefRW<LocalTransform> projectileLt = SystemAPI.GetComponentRW<LocalTransform>(projectileEntity);
                projectileLt.ValueRW.Position = gunLtw.ValueRO.Position + direction * 0.1f;
                projectileLt.ValueRW.Rotation = gunLtw.ValueRO.Rotation;

                RefRW<PhysicsVelocity> projectileVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(projectileEntity);
                projectileVelocity.ValueRW.Linear = direction * 50f;

                RefRW<Projectile> projectile = SystemAPI.GetComponentRW<Projectile>(projectileEntity);
                projectile.ValueRW.Damage = gun.ValueRO.Damage;

                gun.ValueRW.Cooldown = 1f / gun.ValueRO.Rate;
            }

            gun.ValueRW.Cooldown = math.max(gun.ValueRO.Cooldown - Time.deltaTime, 0f);
        }
    }
}
