using Packages.Utils.SoundManager.Signals;
using ShootCommon.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Packages.Utils.SoundManager.Test
{
    [RequireComponent(typeof(Button))]
    public class OnButtonClick : MonoBehaviour
    {
        private ISignalService _signalService;
        private ISoundController _soundController;
        public void Start()
        {
            ProjectContext.Instance.Container.Inject(this);
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        [Inject]
        public void Init(ISignalService signalService,
            ISoundController soundController)
        {
            _signalService = signalService;
            _soundController = soundController;
        }

        public void OnClick()
        {
            _signalService.Publish(new PlayAudioClipAddressableSignal()
            {
                ClipName = "OnClick",
                ClipType = SoundType.Default
            });
        }
    }
}