using Unity.Entities;
using UnityEngine;

public class GunAuthoring : MonoBehaviour
{
    [field: SerializeField] public ProjectileAuthoring ProjectilePrefab { get; private set; }
}

public class GunBaker : Baker<GunAuthoring>
{
    public override void Bake(GunAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new Gun()
        {
            ProjectilePrefab = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic),
            Rate = 10f,
            Cooldown = 0f,
            Damage = 10f,
        });
    }
}