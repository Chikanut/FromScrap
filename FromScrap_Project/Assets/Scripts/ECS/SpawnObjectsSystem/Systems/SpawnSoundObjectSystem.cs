using BovineLabs.Event.Systems;
using Packages.Utils.SoundManager;
using Packages.Utils.SoundManager.Signals;
using SpawnGameObjects.Components;

namespace SpawnGameObjects.Systems
{
    public class SpawnSoundObjectSystem: ConsumeSingleEventSystemBase<SpawnSoundObjectEvent>
    {
        protected override void OnEvent(SpawnSoundObjectEvent e)
        {
            SoundController.Instance.PlayAddressable(new PlayAudioClipAddressableSignal()
            {
                ClipName = e.ClipName.Value, Delay = e.Delay, Pitch = e.Pitch, ClipType = e.ClipType,
                PitchTime = e.PitchTime, Position = e.Position
            });
        }
    }
}