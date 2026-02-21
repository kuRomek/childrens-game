using System;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Configs/Player")]
public class PlayerConfigScriptable : ScriptableObject
{
    [field: SerializeField] public PlayerConfig Data { get; private set; }
}

[Serializable]
public struct PlayerConfig : IComponentData
{
    [Range(0.1f, 10f)] public float DefaultSpeed;
    [Range(0.1f, 10f)] public float MouseSensitivity;
    [Range(0.1f, 30f)] public float JumpForce;
}