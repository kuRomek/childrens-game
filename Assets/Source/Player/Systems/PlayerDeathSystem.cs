using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine.SceneManagement;

[UpdateInGroup(typeof(PresentationSystemGroup))]
partial class PlayerDeathSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        if (SystemAPI.HasSingleton<PlayerDeath>())
        {
            var entity = SystemAPI.GetSingletonEntity<PlayerDeath>();
            LoseGame(SystemAPI.GetComponent<PlayerDeath>(entity).Delay).Forget();
            buffer.DestroyEntity(entity);
        }

        buffer.Playback(World.EntityManager);
        buffer.Dispose();
    }

    private async UniTaskVoid LoseGame(float delay)
    {
        await UniTask.WaitForSeconds(delay);

        SceneManager.LoadScene(1);
    }
}
