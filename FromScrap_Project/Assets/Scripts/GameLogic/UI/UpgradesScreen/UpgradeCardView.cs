using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Upgrades
{
    public class UpgradeCardData
    {
        //public 
    }

    public class UpgradeCardView : MonoBehaviour
    {
        [SerializeField] private Image _backGround;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _frame;
        
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _info;
        [SerializeField] private TextMeshProUGUI _lvl;

        public void Init()
        {
            
        }
    }
}