using System.Linq;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class JumperAuthoring : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        var collidersAuthoring = GetComponentsInChildren<PhysicsShapeAuthoring>().Where(
                collider => collider.gameObject != gameObject &&
                collider.CollisionResponse == CollisionResponsePolicy.RaiseTriggerEvents);

        if (collidersAuthoring.Count() == 0)
            UnityEditor.EditorApplication.delayCall += InitializeGroundDetector;
    }

    private void InitializeGroundDetector()
    {
        UnityEditor.EditorApplication.delayCall -= InitializeGroundDetector;

        GameObject groundDetectorObject = new("GroundDetector", typeof(PhysicsShapeAuthoring));
        groundDetectorObject.transform.SetParent(transform);
        groundDetectorObject.transform.localPosition = default;
        groundDetectorObject.transform.localRotation = default;

        var collider = groundDetectorObject.GetComponent<PhysicsShapeAuthoring>();
        collider.SetBox(new BoxGeometry() { Size = new(0.3f, 0.015f, 0.3f) });
        collider.CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents;

        var collidesWith = collider.CollidesWith;
        collidesWith.Value = 1u << 0;
        collider.CollidesWith = collidesWith;
    }
#endif
}

public class JumperBaker : Baker<JumperAuthoring>
{
    public override void Bake(JumperAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new Jumper()
        {
            IsGrounded = true,
            ReadyToJump = false,
            JumpForce = Configs.Test.JumpForce,
        });
    }
}
