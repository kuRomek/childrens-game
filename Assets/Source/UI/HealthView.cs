using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Slider _sliderBar;

    private EntityManager _em;

    private void Awake()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;

        _sliderBar.minValue = 0f;
        UpdateBar(100f, 100f);
    }

    private void Update()
    {
        EntityQuery queryPlayerHealth = _em.CreateEntityQuery(typeof(Player), typeof(Health));

        if (queryPlayerHealth.TryGetSingleton(out Player player))
        {
            Health playerHealth = _em.GetComponentData<Health>(player.Entity);
            UpdateBar(playerHealth.Current, playerHealth.Max);
        }
        else
        {
            UpdateBar(0f, 1f);
        }
    }

    private void UpdateBar(float currentValue, float maxValue)
    {
        _sliderBar.maxValue = maxValue;
        _sliderBar.value = currentValue;
    }
}