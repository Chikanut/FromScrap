using System;
using System.Collections.Generic;
using MenuNavigation;
using Unity.Entities;
using UnityEngine;

namespace UI.Upgrades
{
    public class UpgradeScreenView : MenuScreen
    {
        [Header("Components")]
        [SerializeField] private RectTransform _cardsHolder;
        
        private Action _onComplete;

        private List<UpgradeCardView> _cards = new List<UpgradeCardView>();

        public Entity CurrentEntity;
        [HideInInspector] public int CurrentLevel;
        
        public void Init(Entity target, int level, Action onComplete)
        {
            CurrentEntity = target;
            CurrentLevel = level;
            
            _onComplete = onComplete;
        }

        public void ShowCard(UpgradeCardData data, Action onClick)
        {
            var newCard = ObjectsPool.Instance.GetObjectOfType<UpgradeCardView>("UpgradeCard", _cardsHolder);
            newCard.Init(data, onClick);
            _cards.Add(newCard);
        }

        public void HideCards()
        {
            _cards.ForEach(card =>
            {
                card.Destroy();
                ObjectsPool.Instance.ReturnObject(card);
            });
            
            _cards.Clear();
        }

        public void Complete()
        {
            _onComplete?.Invoke();
            MenuNavigationController.HideMenuScreen(this);
        }
    }
}