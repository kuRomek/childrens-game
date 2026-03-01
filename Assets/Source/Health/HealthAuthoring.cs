using NaughtyAttributes;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    [SerializeField] private bool _withView;

    [field: ReadOnly, SerializeField] public MeshRenderer HealthFillingObject { get; private set; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (gameObject.scene.IsValid())
            EditorApplication.delayCall += InitializeComponents;
    }

    private void InitializeComponents()
    {
        EditorApplication.delayCall -= InitializeComponents;

        if (_withView)
        {
            if (HealthFillingObject == null)
            {
                var healthBar = (GameObject)PrefabUtility.InstantiatePrefab(Configs.Health.HealthBarPrefab, transform);
                healthBar.transform.localPosition = new(0f, 1.8f, 0f);
                HealthFillingObject = healthBar.GetComponent<MeshRenderer>();
            }
        }
        else if (HealthFillingObject != null)
        {
            DestroyImmediate(HealthFillingObject.gameObject);
        }
#endif
    }
}

public class HealthBaker : Baker<HealthAuthoring>
{
    public override void Bake(HealthAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        Entity barEntity =
            authoring.HealthFillingObject == null ?
            Entity.Null :
            GetEntity(authoring.HealthFillingObject.gameObject, TransformUsageFlags.Dynamic);

        AddComponent(entity, new Health
        {
            Current = 100f,
            Max = 100f,
            BarEntity = barEntity,
        });
    }
}
