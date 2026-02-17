using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct MovingSystem : ISystem
{
    private float _cameraVerticalRotation;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _cameraVerticalRotation = Camera.main.transform.localEulerAngles.x;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<LocalTransform> localTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithPresent<Player>())
        {
            float horizontalRotation = SystemAPI.Time.DeltaTime * Configs.Player.MouseSensitivity * InputController.LookingDelta.x;

            localTransform.ValueRW.Rotation = localTransform.ValueRW.RotateY(horizontalRotation).Rotation;

            float3 direction =
                localTransform.ValueRO.Right() * InputController.MovingDirection.x +
                localTransform.ValueRO.Forward() * InputController.MovingDirection.z;

            localTransform.ValueRW.Position += Configs.Player.DefaultSpeed * SystemAPI.Time.DeltaTime * direction;

            _cameraVerticalRotation = math.clamp(
                _cameraVerticalRotation - InputController.LookingDelta.y * Configs.Player.MouseSensitivity, -90f, 90f);

            Camera.main.transform.position = localTransform.ValueRO.Position + new float3(0f, 1.65f, 0f);
            Camera.main.transform.rotation = localTransform.ValueRO.Rotation;
            Camera.main.transform.localEulerAngles = new Vector3(_cameraVerticalRotation, Camera.main.transform.localEulerAngles.y);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
