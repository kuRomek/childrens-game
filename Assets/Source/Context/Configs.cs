using Unity.Physics.Authoring;
using UnityEngine;

public static class Configs
{
    public static PlayerConfig Player => Resources.Load<PlayerConfigScriptable>(nameof(Player)).Data;
    public static TestConfig Test => Resources.Load<TestConfigScriptable>(nameof(Test)).Data;
    public static HealthConfig Health => Resources.Load<HealthConfigScriptable>(nameof(Health)).Data;

    public static PhysicsCategoryNames PhysicsCategoryNames => Resources.Load<PhysicsCategoryNames>(nameof(PhysicsCategoryNames));
}
