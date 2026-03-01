using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using UnityEngine;

[RequireComponent(typeof(PhysicsShapeAuthoring), typeof(PhysicsBodyAuthoring), typeof(HealthAuthoring))]
public class UnitAuthoring : MonoBehaviour
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

        var face = GetComponentInChildren<UnitFaceAuthoring>();

        if (face == null)
        {
            var faceObject = new GameObject("Face", typeof(UnitFaceAuthoring));
            faceObject.transform.SetParent(transform);
            faceObject.transform.localPosition = Vector3.up * 1.2f;
            faceObject.transform.rotation = default;
        }

        GetComponent<PhysicsShapeAuthoring>().SetCapsule(new CapsuleGeometryAuthoring()
        {
            Orientation = quaternion.EulerXYZ(math.radians(new float3(90f, 0f, 0f))),
            Center = new float3(0f, 0.7f, 0f),
            Height = 1.4f,
            Radius = 0.3f
        });

        PhysicsBodyAuthoring physicsBody = GetComponent<PhysicsBodyAuthoring>();

        physicsBody.GravityFactor = 3f;
        physicsBody.Smoothing = BodySmoothing.Interpolation;
        physicsBody.OverrideDefaultMassDistribution = true;
        var massDistribution = physicsBody.CustomMassDistribution;
        massDistribution.InertiaTensor = new float3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        physicsBody.CustomMassDistribution = massDistribution;
    }
#endif
}

public class UnitBaker : Baker<UnitAuthoring>
{
    public override void Bake(UnitAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        Entity faceEntity = Entity.Null;

        UnitFaceAuthoring faceAuthoring = GetComponentInChildren<UnitFaceAuthoring>();

        if (faceAuthoring != null)
            faceEntity = GetEntity(faceAuthoring.gameObject, TransformUsageFlags.Dynamic);

        AddComponent(entity, new Moving()
        {
            FaceEntity = faceEntity,
            DefaultSpeed = Configs.Test.DefaultSpeed,
            Direction = default,
            LookingDelta = default,
            VerticalRotation = faceAuthoring.transform.localEulerAngles.x,
            Sprinting = false,
            IsGrounded = true,
        });
    }
}
