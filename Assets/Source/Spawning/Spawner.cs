using Unity.Entities;

public struct Spawner : IComponentData
{
    public float Rate;
    public float CountLeft;
    public float AccumSeconds;
    public Entity PatrolCircle;
    public Entity EnemyPrefabEntity;
}