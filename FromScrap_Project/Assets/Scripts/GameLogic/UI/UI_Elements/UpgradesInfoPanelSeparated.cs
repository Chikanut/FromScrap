using System.Collections.Generic;
using Kits.Components;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesInfoPanelSeparated : UpgradesInfoPanelBase
{
    [SerializeField] private RectTransform _leftPanel;
    [SerializeField] private RectTransform _rightPanel;
    [SerializeField] List<KitType> _leftKitsTypes = new List<KitType>();
    [SerializeField] private GameObject _separator;

    [SerializeField] private RectTransform[] _updateLayouts;
    
    public override void UpdateInfo(CurrentCarInfoData carInfo)
    {
        _separator.SetActive(false);
        base.UpdateInfo(carInfo);
    }

    public override void Visualise(List<UpgradeInfo> info)
    {
        for (var i = 0; i < info.Count; i++)
        {
            var isLeft = _leftKitsTypes.Contains(info[i].KitInfo.Type);
            var parent = isLeft ? _leftPanel : _rightPanel;
            
            if(!isLeft)
                _separator.SetActive(true);
            
            var icon = GetNextIcon(parent);
            UpdateIconInfo(icon, info[i]);
        }

        for (int i = 0; i < _updateLayouts.Length; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_updateLayouts[i]);
        }

        Canvas.ForceUpdateCanvases();
        
    }
}
