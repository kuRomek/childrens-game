using Unity.Entities;

[UpdateInGroup(typeof(PresentationSystemGroup))]
partial class PlayerHealthViewSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (playerHealthViewUpdate, entity) in SystemAPI.Query<RefRO<PlayerHealthViewUpdate>>().WithEntityAccess())
        {
            HealthView.UpdateBar(playerHealthViewUpdate.ValueRO.CurrentValue, playerHealthViewUpdate.ValueRO.MaxValue);
            buffer.DestroyEntity(entity);
        }

        buffer.Playback(World.EntityManager);
        buffer.Dispose();
    }
}
