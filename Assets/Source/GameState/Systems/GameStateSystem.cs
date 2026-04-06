using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

partial class GameStateSystem : SystemBase
{
    protected override void OnCreate()
    {
        World.EntityManager.CreateSingleton<GameState>();
    }

    protected override void OnUpdate()
    {
        if (SystemAPI.HasSingleton<RunStart>())
        {
            var entity = SystemAPI.GetSingletonEntity<RunStart>();
            StartRun(entity);
        }

        if (SystemAPI.HasSingleton<RunEnd>() && SystemAPI.TryGetSingleton(out GameState gameState) && gameState.IsInRun)
        {
            var entity = SystemAPI.GetSingletonEntity<RunEnd>();
            LoseRun(SystemAPI.GetComponent<RunEnd>(entity).Delay, entity).Forget();
        }
    }

    private async UniTaskVoid LoseRun(float delay, Entity runEndEntity)
    {
        InputController.Disable();
        SystemAPI.GetSingletonRW<GameState>().ValueRW.IsInRun = false;

        await UniTask.WaitForSeconds(delay);

        World.EntityManager.DestroyEntity(runEndEntity);
        InputController.Enable();
        SceneManager.LoadScene(1);
    }

    private void StartRun(Entity runStartEntity)
    {
        InputController.Enable();
        SystemAPI.GetSingletonRW<GameState>().ValueRW.IsInRun = true;

        World.EntityManager.DestroyEntity(runStartEntity);
    }
}
