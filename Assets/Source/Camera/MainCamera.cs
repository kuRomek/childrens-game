using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private EntityManager _em;
    private float _verticalRotation;
    private Entity _playerEntity;
    private float3 _playerOffset;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        _verticalRotation = transform.localEulerAngles.x;

        EntityQuery queryPlayer = _em.CreateEntityQuery(typeof(Player), typeof(LocalTransform));

        _playerEntity = queryPlayer.GetSingleton<Player>().Entity;
        LocalTransform playerTransform = _em.GetComponentData<LocalTransform>(_playerEntity);

        _playerOffset = (float3)transform.position - playerTransform.Position;
    }

    private void LateUpdate()
    {
        EntityQuery queryInput = _em.CreateEntityQuery(typeof(PlayerInput));
        PlayerInput input = queryInput.GetSingleton<PlayerInput>();
        LocalTransform playerTransform = _em.GetComponentData<LocalTransform>(_playerEntity);

        float lookingDelta = Configs.Player.MouseSensitivity * Time.deltaTime * math.degrees(input.LookingDelta.y);

        _verticalRotation = math.clamp(_verticalRotation - lookingDelta, -90f, 90f);

        transform.SetPositionAndRotation(playerTransform.Position + _playerOffset, playerTransform.Rotation);
        transform.localEulerAngles = new(_verticalRotation, transform.localEulerAngles.y);
    }
}
