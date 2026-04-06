using System;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "Test", menuName = "Configs/Test")]
public class TestConfigScriptable : ScriptableObject
{
    [field: SerializeField] public TestConfig Data { get; private set; }
}

[Serializable]
public struct TestConfig : IComponentData
{
    [Range(0.1f, 10f)] public float Speed;
    [Range(0.1f, 80f)] public float Acceleration;
    [Range(0.1f, 10f)] public float MouseSensitivity;
    [Range(0.1f, 30f)] public float JumpForce;
    [Range(0.1f, 1000f)] public float Damage;
    [Range(0.1f, 30f)] public float DamageForce;
    [Range(0.1f, 100f)] public float GunRate;
    [Range(0.1f, 5f)] public float SprintingSpeedMultiplier;
    [Range(0.01f, 100f)] public float EngagementFadeSpeed;
    [Range(0.01f, 100f)] public float EngagementBurstForKilledEnemy;
}