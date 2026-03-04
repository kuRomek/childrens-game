using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PhysicsShapeAuthoring), typeof(PhysicsBodyAuthoring))]
public class ProjectileAuthoring : MonoBehaviour
{
    private void OnValidate()
    {
        EditorApplication.delayCall += InitializeComponents;
    }

    private void InitializeComponents()
    {
        EditorApplication.delayCall -= InitializeComponents;


    }
}

public class ProjectileBaker : Baker<ProjectileAuthoring>
{
    public override void Bake(ProjectileAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Projectile() { LifeSpan = 1f });
    }
}