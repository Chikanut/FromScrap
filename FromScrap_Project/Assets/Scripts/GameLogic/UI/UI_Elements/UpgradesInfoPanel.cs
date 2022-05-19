using System.Collections.Generic;
using System.Linq;
using Cars.View.Components;
using Kits.Components;
using Packages.Common.Storage.Config.Cars;
using Packages.Common.Storage.Config.Upgrades;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class UpgradesInfoPanel : MonoBehaviour
{
    [Header("Components")] [SerializeField]
    private RectTransform _rowHolder;

    [SerializeField] private UpgradeIconView _upgradePrefab;

    [Header("Settings")] [SerializeField] private int _maxItemsInRow = 2;
    [SerializeField] List<KitType> _showKitTypes = new List<KitType>();
    
    public enum PanelType
    {
        upgrade,
        placeholder
    }

    public class UpgradeInfo
    {
        public PanelType Type;
        public KitInfoData KitInfo;
        public int Level;
    }


    List<GameObject> _placeHolders = new List<GameObject>();
    List<UpgradeIconView> _icons = new List<UpgradeIconView>();
    List<RectTransform> _rows = new List<RectTransform>();
    
    public void UpdateInfo(CurrentCarInfoData carInfo)
    {
        ClearAll();

       var info = GetInfo(carInfo);
       Visualise(info);
    }

    List<UpgradeInfo> GetInfo(CurrentCarInfoData carInfo)
    {
        var kits = new List<UpgradeInfo>();

        foreach (var platform in carInfo.platformInfos)
        {
            var platformConnectedKits = platform.ConnectedKits;
          
            var kitsIDs = platform.ConnectedKitsIDs;

            var findedKitInside = false;

            for (var j = 0; j < platformConnectedKits.Count; j++)
            {
                var kit = platformConnectedKits[j];
                var kitID = kitsIDs[j];

                if (!_showKitTypes.Contains(kit.Type)) continue;
                
                kits.Add(new UpgradeInfo()
                {
                    KitInfo = carInfo.carData.UpgradesConfigs[kitID.ID],
                    Level = kit.KitLevel,
                    Type = PanelType.upgrade
                });
                
                findedKitInside = true;
            }

            if (findedKitInside) continue;
            
            var platformConnections = platform.Connections;
            var targetPlatform = platformConnections.Any(connection => _showKitTypes.Contains(connection));

            if (targetPlatform)
                kits.Add(new UpgradeInfo() {Type = PanelType.placeholder});
        }

        return kits;
    }

    void Visualise(List<UpgradeInfo> info)
    {
        var rowsInfo = GetLinesCount(info.Count);

        var iconCount = 1;
        
        for (int i = 0; i < rowsInfo.linesCount; i++)
        {
            var row = GetNextRow();

            for (int j = 0; j < iconCount; j++)
            {

                if (info.Count > 0)
                {
                    var infoItem = info[0];
                    var icon = GetNextIcon(row);
                    icon.Reset();
                    
                    if (infoItem.Type == PanelType.upgrade)
                    {
                        icon.Init(infoItem.KitInfo.Icon);
                        icon.SetState(UpgradeIconView.UpgradeIconState.active);
                        icon.ShowUpgrades(infoItem.Level, false);
                    }
                    else
                    {
                        icon.SetState(UpgradeIconView.UpgradeIconState.disabled);
                    }
                    
                    info.RemoveAt(0);
                }
                else
                {
                    GetNextPlaceHolder(row);
                }
            }

            iconCount++;
            
            if(iconCount > _maxItemsInRow)
                iconCount = 1;
        }
    }

    public (int linesCount, int placeholdersCount) GetLinesCount(int elementsCount)
    {
        var n = elementsCount;
        var elementsInLine = 1;
        var linesCount = 0;
        
        while (n > 0)
        {
            n -= elementsInLine;
            
            if (elementsInLine >= _maxItemsInRow)
                elementsInLine = 0;

            elementsInLine++;
            
            linesCount++;
        }

        return (linesCount, math.abs(n));
    }

    public void ClearAll()
    {
        _placeHolders.ForEach(o => o.SetActive(false));
        _rows.ForEach(o => o.gameObject.SetActive(false));
        _icons.ForEach(o => o.gameObject.SetActive(false));
    }

    RectTransform GetNextRow()
    {
        foreach (var row in _rows)
        {
            if (row.gameObject.activeSelf != false) continue;

            row.SetAsLastSibling();
            row.gameObject.SetActive(true);

            return row;
        }

        var newRow = Instantiate(_rowHolder, _rowHolder.parent);
        newRow.SetAsLastSibling();
        _rows.Add(newRow);

        return newRow;
    }

    UpgradeIconView GetNextIcon(Transform parent)
    {
        foreach (var icon in _icons)
        {
            if (icon.gameObject.activeSelf) continue;

            icon.gameObject.SetActive(true);
            icon.transform.SetParent(parent);

            return icon;
        }

        var newIcon = Instantiate(_upgradePrefab, parent);
        newIcon.gameObject.SetActive(true);
        _icons.Add(newIcon);

        return newIcon;
    }

    GameObject GetNextPlaceHolder(Transform parent)
    {
        foreach (var placeHolder in _placeHolders)
        {
            if (placeHolder.activeSelf != false) continue;

            placeHolder.SetActive(true);
            placeHolder.transform.SetParent(parent);

            return placeHolder;
        }

        var newPlaceHolder = new GameObject("Placeholder", typeof(RectTransform));
        newPlaceHolder.transform.SetParent(parent);
        newPlaceHolder.SetActive(true);
        _placeHolders.Add(newPlaceHolder);

        return newPlaceHolder;
    }
}
