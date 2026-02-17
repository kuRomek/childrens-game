using Unity.Entities;
using UnityEngine;

public class PlayerAuthorizer : MonoBehaviour
{
    public class Baker : Baker<PlayerAuthorizer>
    {
        public override void Bake(PlayerAuthorizer authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Player());
        }
    }
}
