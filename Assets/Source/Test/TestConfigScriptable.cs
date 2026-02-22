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
    [Range(0.1f, 10f)] public float DefaultSpeed;
    [Range(0.1f, 10f)] public float MouseSensitivity;
    [Range(0.1f, 30f)] public float JumpForce;
}