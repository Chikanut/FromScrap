using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Sounds Config", menuName = "Configs/Audio")]
public class SoundsConfig : ScriptableObject {
    [SerializeField]
    public ClipsDictionary ClipsDictionary = new ClipsDictionary();

}

[System.Serializable]
public class AudioClipConfig 
{
    public AssetReference[] Reference;
    public float Volume;
    public AudioMixerGroup OutputAudioMixerGroup;
    public int MaxSimultaneouslyCalls = 4;
    public AssetReference GetReference()
    {
        return Reference[Random.Range(0, Reference.Length)];
    }
}

[System.Serializable]
public class ClipsDictionary : SerializableDictionaryBase<string, AudioClipConfig> { }