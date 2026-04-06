using Unity.Entities;
using UnityEngine;

public class PatrolCircleAuthoring : MonoBehaviour
{
    [field: SerializeField] public float Radius { get; private set; }
}

public class PatrolCircleBacker : Baker<PatrolCircleAuthoring>
{
    public override void Bake(PatrolCircleAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.WorldSpace);
        AddComponent(entity, new PatrolCircle() { Radius = authoring.Radius, });
    }
}