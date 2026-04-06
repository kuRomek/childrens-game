using Unity.Entities;
using UnityEngine;

public class UnitFaceAuthoring : MonoBehaviour { }

public class UnitFaceBaker : Baker<UnitFaceAuthoring>
{
    public override void Bake(UnitFaceAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<UnitFace>(entity);
    }
}