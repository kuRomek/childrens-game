using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

[RequireComponent(typeof(PhysicsShapeAuthoring), typeof(PhysicsBodyAuthoring))]
public class GroundAuthoring : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (gameObject.scene.IsValid())
            UnityEditor.EditorApplication.delayCall += InitializeComponents;
    }

    private void InitializeComponents()
    {
        UnityEditor.EditorApplication.delayCall -= InitializeComponents;

        GetComponent<PhysicsBodyAuthoring>().MotionType = BodyMotionType.Static;
    }
#endif
}

public class GroundBaker : Baker<GroundAuthoring>
{
    public override void Bake(GroundAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new Ground());
    }
}
