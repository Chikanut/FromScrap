using System.Collections.Generic;
using Kits.Components;
using Packages.Common.Storage.Config.Upgrades;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesInfoPanelSeparated : UpgradesInfoPanelBase
{
    [SerializeField] private RectTransform _leftPanel;
    [SerializeField] private RectTransform _rightPanel;
    [SerializeField] List<KitType> _leftKitsTypes = new List<KitType>();
    [SerializeField] private GameObject _separator;

    [SerializeField] private List<LayoutGroup> _layoutGroups = new List<LayoutGroup>();
    [SerializeField] private List<ContentSizeFitter> _sizeFitters = new List<ContentSizeFitter>();
    
    public override void UpdateInfo(CurrentCarInfoData carInfo, UpgradesConfigData upgradesConfigData)
    {
        _separator.SetActive(false);
        base.UpdateInfo(carInfo, upgradesConfigData);
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
        
        Canvas.ForceUpdateCanvases();
        
        // for (int i = 0; i < _updateLayouts.Length; i++)
        // {
        //     // LayoutRebuilder.ForceRebuildLayoutImmediate(_updateLayouts[i].rec);
        //
        //     _updateLayouts[i].enabled = false;
        //     _updateLayouts[i].enabled = true;
        //     
        //     _updateLayouts[i].CalculateLayoutInputHorizontal();
        //     _updateLayouts[i].CalculateLayoutInputVertical();
        //     
        //     LayoutRebuilder.ForceRebuildLayoutImmediate(_updateLayouts[i].GetComponent<RectTransform>());
        //     
        //
        // }
        
        _sizeFitters.ForEach(group=>group.enabled = false);
        _layoutGroups.ForEach(group=>group.enabled = false);
        _layoutGroups.ForEach(group=>group.enabled = true);
        _sizeFitters.ForEach(group=>group.enabled = true);
        
    }
}
