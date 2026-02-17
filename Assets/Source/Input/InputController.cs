using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private static InputController _instance;

    private PlayerInput _input;

    public static float3 MovingDirection { get; private set; }
    public static float2 LookingDelta { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(_instance.gameObject);

        _instance = this;

        _input = new();
    }

    private void OnEnable()
    {
        _input.Enable();

        _input.Player.Move.performed += OnMovePerformed;
        _input.Player.Move.canceled += OnMovePerformed;

        _input.Player.Look.performed += OnLookPerformed;
        _input.Player.Look.canceled += OnLookPerformed;
    }

    private void OnDisable()
    {
        _input.Player.Move.performed -= OnMovePerformed;
        _input.Player.Move.canceled -= OnMovePerformed;

        _input.Player.Look.performed -= OnLookPerformed;
        _input.Player.Look.canceled -= OnLookPerformed;

        _input.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<Vector2>();
        MovingDirection = new(direction.x, 0f, direction.y);
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        var lookingDelta = context.ReadValue<Vector2>();
        LookingDelta = new float2(lookingDelta.x, lookingDelta.y);
    }
}
