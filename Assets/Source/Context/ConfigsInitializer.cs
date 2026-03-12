using Unity.Entities;
using UnityEngine;

public class ConfigsInitializer : Installer
{
    public override void Install()
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;

        em.SetComponentData(em.CreateSingleton<PlayerConfig>(), Configs.Player);
        em.SetComponentData(em.CreateSingleton<TestConfig>(), Configs.Test);
        //em.SetComponentData(em.CreateSingleton<HealthConfig>(), Configs.Health);
        em.SetComponentData(em.CreateSingleton<SpawningConfig>(), Configs.Spawning);
    }
}
