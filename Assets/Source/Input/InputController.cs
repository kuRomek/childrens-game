using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private PlayerInputActions _input;
    private Entity _inputEntity;

    private void Awake()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _inputEntity = entityManager.CreateSingleton<PlayerInput>();

        _input = new();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Update()
    {
        float2 movingDirection = _input.Player.Move.ReadValue<Vector2>();
        float2 lookingDelta = _input.Player.Look.ReadValue<Vector2>();
        bool hasJumped = _input.Player.Jump.WasPressedThisFrame();
        bool sprinting = _input.Player.Sprint.IsPressed();

        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        entityManager.SetComponentData(_inputEntity, new PlayerInput()
        {
            MovingDirection = new(movingDirection.x, 0f, movingDirection.y),
            LookingDelta = new float2(lookingDelta.x, lookingDelta.y),
            HasJumped = hasJumped,
            Sprinting = sprinting
        });
    }
}
