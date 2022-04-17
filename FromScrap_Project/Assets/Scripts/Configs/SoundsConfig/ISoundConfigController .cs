namespace Packages.Common.Storage.Config
{
    public interface ISoundConfigController 
    {
        SoundConfigData GetSoundData { get; }
        void SetInfo(SoundsConfig data);
    }
}