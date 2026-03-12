using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

public class DebugVisual : Installer
{
    [BoxGroup("DamageDebug")]
    [SerializeField, Range(1f, 300f)] private float _damageAmount;

    [BoxGroup("DamageDebug")]
    [SerializeField] private int _entityId;

    [ReadOnly, SerializeField] private bool _healthBarsShown;

    public override void Install()
    {

    }

    [Button]
    private void ToggleHealthBars()
    {
        _healthBarsShown = !_healthBarsShown;
    }

    [Button]
    private void DealDamage()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;

        var entities = em.GetAllEntities();

        foreach (var entity in entities)
        {
            if (entity.Index == _entityId)
            {
                Entity damageEntity = em.CreateEntity(typeof(Damage));
                em.SetComponentData(damageEntity, new Damage() { Amount = _damageAmount, SubjectEntity = entity });
            }
        }

        entities.Dispose();
    }
}
