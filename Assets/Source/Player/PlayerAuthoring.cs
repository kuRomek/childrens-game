using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(UnitAuthoring), typeof(JumperAuthoring))]
public class PlayerAuthoring : MonoBehaviour { }

public class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Player() { Entity = entity });
    }
}
