using System;
using System.Collections.Generic;
using I2.Loc;
using Packages.Common.Storage.Config.Upgrades;
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
        public List<KitComponentData.Description> Descriptions;
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
        [SerializeField] private UpgradeIconView _icon;
        [Space]
        [SerializeField] private TextMeshProUGUI _info;
        [Space]
        [SerializeField] private Button _button;

        [Space]
        [Header("Resources")] 
        [SerializeField] private BackgroundsDictionary _backgrounds = new BackgroundsDictionary();


        private Action _onClick;
        
        void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        public void Init(UpgradeCardData data, Action onClick)
        {
            if (_backgrounds.ContainsKey(data.Type))
                _backGround.sprite = _backgrounds[data.Type];

            _icon.Reset();
            _icon.Init(data.Icon, data.NameKey);
            _icon.ShowUpgrades(data.UpgradeLevel, true);

            var descriptionText ="";
            
            foreach (var description in data.Descriptions)
            {
                var descriptionTranslation = LocalizationManager.GetTranslation(description.DescriptionKey);
                if (!string.IsNullOrEmpty(descriptionText))
                {
                    for (var i = 0; i < description.Values.Length; i++)
                        descriptionTranslation =
                            descriptionTranslation.Replace("{" + i + "}", description.Values[i].ToString());
                }

                descriptionText += descriptionTranslation + "\n";
            }

            _info.text = descriptionText;

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