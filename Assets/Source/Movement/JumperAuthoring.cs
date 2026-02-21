using System.Linq;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class JumperAuthoring : MonoBehaviour
{
    private void OnValidate()
    {
        var collidersAuthoring = GetComponentsInChildren<PhysicsShapeAuthoring>().
            Where(collider => collider.gameObject != gameObject);

        if (collidersAuthoring.Count() == 0)
        {
            GameObject groundDetectorObject = new("GroundDetector", typeof(PhysicsShapeAuthoring));
            groundDetectorObject.transform.SetParent(transform);
            groundDetectorObject.transform.localPosition = default;
            groundDetectorObject.transform.localRotation = default;

            var collider = groundDetectorObject.GetComponent<PhysicsShapeAuthoring>();
            collider.SetBox(new BoxGeometry() { Size = new(0.3f, 0.015f, 0.3f) });
            collider.CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents;
        }
    }
}

public class JumperBaker : Baker<JumperAuthoring>
{
    public override void Bake(JumperAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Jumper() { IsGrounded = true });
    }
}
