using UnityEngine;

public static class ContextLoader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Load()
    {
        if (Object.FindFirstObjectByType<Context>() != null)
            return;

        Object.Instantiate(Resources.Load<Context>(nameof(Context))).Initialize();
    }
}
