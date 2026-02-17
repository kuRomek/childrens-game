using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Configs/Player")]
public class PlayerConfig : ScriptableObject
{
    [field: SerializeField, Range(0.1f, 10f)] public float DefaultSpeed { get; private set; } = 3f;
    [field: SerializeField, Range(0.1f, 10f)] public float MouseSensitivity { get; private set; } = 0.4f;
}
