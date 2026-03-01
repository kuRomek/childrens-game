using System;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawning", menuName = "Configs/Spawning")]
public class SpawningConfigScriptable : ScriptableObject
{
    [field: SerializeField] public SpawningConfig Data { get; private set; }
}

[Serializable]
public struct SpawningConfig : IComponentData
{
    [Range(0f, 10f)] public float Rate;
    [Range(0, 10)] public int CountLeft;
}