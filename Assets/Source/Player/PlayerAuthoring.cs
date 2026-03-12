using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

[RequireComponent(typeof(UnitAuthoring), typeof(JumperAuthoring))]
public class PlayerAuthoring : MonoBehaviour
{
    private void OnValidate()
    {
        if (gameObject.scene.IsValid())
            UnityEditor.EditorApplication.delayCall += InitializeComponents;
    }

    private void InitializeComponents()
    {
        UnityEditor.EditorApplication.delayCall -= InitializeComponents;

        var collider = GetComponent<PhysicsShapeAuthoring>();
    }
}

public class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Player() { Entity = entity });
    }
}
