using System;
using Unity.Entities;

public struct GroundInteractionPair : IEquatable<GroundInteractionPair>
{
    public Entity JumperEntity;
    public Entity GroundEntity;

    public bool Equals(GroundInteractionPair other) =>
        JumperEntity == other.JumperEntity && GroundEntity == other.GroundEntity;

    public override int GetHashCode() =>
        JumperEntity.GetHashCode() ^ GroundEntity.GetHashCode();
}