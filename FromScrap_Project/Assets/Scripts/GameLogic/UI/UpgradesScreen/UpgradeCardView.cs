using System;
using System.Collections.Generic;
using I2.Loc;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Upgrades
{
    public enum UpgradeCardType
    {
        _replace,
        _new,
        _upgrade,
        _gold,
        _health
    }

    public class UpgradeCardData
    {
        public string NameKey;
        public string DescriptionKey;
        public Sprite Icon;
        public UpgradeCardType Type;
        public int UpgradeLevel;
    }

    public class UpgradeCardView : PoolObject
    {
        [System.Serializable]
        public class BackgroundsDictionary : SerializableDictionaryBase<UpgradeCardType, Sprite> { }
        
        [Header("Components")]
        [SerializeField] private Image _backGround;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _frame;
        [Space]
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _info;
        [SerializeField] private TextMeshProUGUI _lvl;
        [Space]
        [SerializeField] private Button _button;

        [Space]
        [Header("Resources")] 
        [SerializeField] private BackgroundsDictionary _backgrounds = new BackgroundsDictionary();
        [SerializeField] private List<Sprite> _frames = new List<Sprite>();


        private Action _onClick;
        
        void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        public void Init(UpgradeCardData data, Action onClick)
        {
            if (_backgrounds.ContainsKey(data.Type))
                _backGround.sprite = _backgrounds[data.Type];
            
            if (_frames.Count > 0)
                _frame.sprite = _frames[Mathf.Clamp(data.UpgradeLevel, 0, _frames.Count - 1)];
            
            _icon.sprite = data.Icon;
            _name.text = LocalizationManager.GetTranslation("Kits/"+data.NameKey);
            _info.text = LocalizationManager.GetTranslation("Kits/"+data.DescriptionKey);
            _lvl.text = string.Format(LocalizationManager.GetTranslation("_lvl") + " {0}", data.UpgradeLevel);
            
            _onClick = onClick;
        }

        void OnClick()
        {
            _onClick?.Invoke();
        }

        public override void AcceptObjectsLinks(List<PoolObject> objects) { }

        public override void ResetState()
        {
            _onClick = null;
        }
    }
}