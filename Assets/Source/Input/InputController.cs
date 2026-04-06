using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class InputController : Installer
{
    private static InputController _instance;

    private EntityManager _em;
    private PlayerInputActions _input;
    private Entity _inputEntity;
    private bool _isGamePaused;

    public override void Install()
    {
        if (_instance != null && _instance != this)
        {
            transform.SetParent(_instance.transform.parent);
            Destroy(_instance.gameObject);
        }

        _instance = this;

        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        _inputEntity = _em.CreateSingleton<PlayerInput>();

        _input = new();
        _input.Enable();
    }

    private void Update()
    {
        if (_isGamePaused)
            return;

        float2 movingDirection = _input.Player.Move.ReadValue<Vector2>();
        float2 lookingDelta = _input.Player.Look.ReadValue<Vector2>();
        bool hasJumped = _input.Player.Jump.WasPressedThisFrame();
        bool sprinting = _input.Player.Sprint.IsPressed();
        bool shooting = _input.Player.Attack.IsPressed();
        bool hasInteracted = _input.Player.Interact.WasPressedThisFrame();

        _em.SetComponentData(_inputEntity, new PlayerInput()
        {
            MovingDirection = new(movingDirection.x, 0f, movingDirection.y),
            LookingDelta = new float2(lookingDelta.x, lookingDelta.y),
            HasJumped = hasJumped,
            Sprinting = sprinting,
            Shooting = shooting,
            HasInteracted = hasInteracted,
        });
    }

    private void Start()
    {
        Enable();
    }

    private void OnDestroy()
    {
        _input.Disable();
    }

    public static void Enable()
    {
        _instance._isGamePaused = false;
    }

    public static void Disable()
    {
        _instance._em.SetComponentData(_instance._inputEntity, new PlayerInput()
        {
            MovingDirection = default,
            LookingDelta = default,
            HasJumped = false,
            Sprinting = false,
            Shooting = false,
            HasInteracted = false,
        });

        _instance._isGamePaused = true;
    }
}
