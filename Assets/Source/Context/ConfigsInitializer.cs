using Unity.Entities;
using UnityEngine;

public class ConfigsInitializer : MonoBehaviour
{
    private void Awake()
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;

        em.SetComponentData(em.CreateSingleton<PlayerConfig>(), Configs.Player);
        em.SetComponentData(em.CreateSingleton<TestConfig>(), Configs.Test);
    }
}
