using System;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "Configs/Health")]
public class HealthConfigScriptable : ScriptableObject
{
    [field: SerializeField] public HealthConfig Data { get; private set; }
}

[Serializable]
public struct HealthConfig
{
    public GameObject HealthBarPrefab;
}