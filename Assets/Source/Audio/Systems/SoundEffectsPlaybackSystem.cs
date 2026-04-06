using Unity.Entities;

[UpdateInGroup(typeof(PresentationSystemGroup))]
partial class SoundEffectsPlaybackSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (soundEffect, entity) in SystemAPI.Query<RefRO<SoundEffect>>().WithEntityAccess())
        {
            AudioPlayer.PlaySoundEffect(soundEffect.ValueRO).Forget();
            buffer.DestroyEntity(entity);
        }

        buffer.Playback(World.EntityManager);
        buffer.Dispose();
    }
}
