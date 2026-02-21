using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : UnitAuthoring { }

public class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Moving());
        AddComponent(entity, new Player() { Entity = entity });
    }
}
