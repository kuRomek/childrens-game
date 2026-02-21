using Unity.Entities;
using UnityEngine;

public class GroundAuthoring : MonoBehaviour { }

public class GroundBaker : Baker<GroundAuthoring>
{
    public override void Bake(GroundAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new Ground());
    }
}
