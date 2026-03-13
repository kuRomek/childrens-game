using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private EntityManager _em;
    private Door _currentHighlighting = null;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        Ray ray = _camera.ViewportPointToRay(new(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, 2f) && hit.collider.TryGetComponent(out Door door))
        {
            if (_currentHighlighting != door && _currentHighlighting != null)
                _currentHighlighting.ToggleHighlight(false);

            _currentHighlighting = door;
            _currentHighlighting.ToggleHighlight(true);

            var inputQuery = _em.CreateEntityQuery(typeof(PlayerInput));

            if (inputQuery.TryGetSingleton(out PlayerInput input) && input.HasInteracted)
                door.Interact();
        }
        else if (_currentHighlighting != null)
        {
            _currentHighlighting.ToggleHighlight(false);
            _currentHighlighting = null;
        }
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
