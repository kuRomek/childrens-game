using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private EntityManager _em;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void LateUpdate()
    {
        EntityQuery queryPlayer = _em.CreateEntityQuery(typeof(Player), typeof(Moving));

        if (queryPlayer.TryGetSingleton(out Player player))
        {
            var playerFaceEntity = _em.GetComponentData<Moving>(player.Entity).FaceEntity;
            var localToWorld = _em.GetComponentData<LocalToWorld>(playerFaceEntity);

            transform.SetPositionAndRotation(localToWorld.Position, localToWorld.Rotation);
        }
    }
}
