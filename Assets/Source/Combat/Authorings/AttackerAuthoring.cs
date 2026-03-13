using Unity.Entities;
using UnityEngine;

public class AttackerAuthoring : MonoBehaviour
{
    [field: SerializeField] public GameObject WeaponGameObject { get; private set; }
}

public class AttackerBaker : Baker<AttackerAuthoring>
{
    public override void Bake(AttackerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.WorldSpace);
        AddComponent(entity, new Attacker()
        {
            Attacking = false,
            WeaponEntity = GetEntity(authoring.WeaponGameObject, TransformUsageFlags.Dynamic),
        });
    }
}