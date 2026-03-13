using System;
using Unity.Entities;

public struct EntityOrderedPair : IEquatable<EntityOrderedPair>
{
    public Entity Entity1;
    public Entity Entity2;

    public bool Equals(EntityOrderedPair other) =>
        Entity1 == other.Entity1 && Entity2 == other.Entity2;

    public override int GetHashCode() =>
        Entity1.GetHashCode() ^ Entity2.GetHashCode();
}