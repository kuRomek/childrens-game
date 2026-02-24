using Unity.Entities;

public struct Damage : IComponentData
{
    public Entity SubjectEntity;
    public float Amount;
}
