using Unity.Entities;
using Unity.Mathematics;

public struct SoundEffect : IComponentData
{
    public AudioKeys.Sound SoundKey;
    public float2 PitchRange;
    public float3 Position;
}