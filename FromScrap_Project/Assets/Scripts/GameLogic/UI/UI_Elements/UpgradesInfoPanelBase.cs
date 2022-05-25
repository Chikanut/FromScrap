using System.Collections.Generic;
using System.Linq;
using Kits.Components;
using Packages.Common.Storage.Config.Upgrades;
using UnityEngine;

public class UpgradesInfoPanelBase : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform _rowHolder;
    [SerializeField] private UpgradeIconView _upgradePrefab;
    [SerializeField] List<KitType> _showKitTypes = new List<KitType>();
    [SerializeField] private bool _includeKitPlatforms = false;

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

    public virtual void UpdateInfo(CurrentCarInfoData carInfo)
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

            if (findedKitInside || !_includeKitPlatforms) continue;

            var platformConnections = platform.Connections;
            var targetPlatform = platformConnections.Any(connection => _showKitTypes.Contains(connection));

            if (targetPlatform)
                kits.Add(new UpgradeInfo() {Type = PanelType.placeholder});
        }

        return kits;
    }

    public virtual void Visualise(List<UpgradeInfo> info)
    {

    }

    protected void UpdateIconInfo(UpgradeIconView icon, UpgradeInfo info)
    {
        icon.Reset();

        if (info.Type == PanelType.upgrade)
        {
            icon.Init(info.KitInfo.Icon);
            icon.SetState(UpgradeIconView.UpgradeIconState.active);
            icon.ShowUpgrades(info.Level, false);
        }else
        {
            icon.SetState(UpgradeIconView.UpgradeIconState.disabled);
        }
    }

    public void ClearAll()
    {
        _placeHolders.ForEach(o => o.SetActive(false));
        _rows.ForEach(o => o.gameObject.SetActive(false));
        _icons.ForEach(o => o.gameObject.SetActive(false));
    }

    public RectTransform GetNextRow()
    {
        foreach (var row in _rows)
        {
            if (row.gameObject.activeSelf != false) continue;

            row.SetAsLastSibling();
            row.gameObject.SetActive(true);

            return row;
        }

        var newRow = Instantiate(_rowHolder, _rowHolder.parent);
        newRow.gameObject.SetActive(true);
        newRow.SetAsLastSibling();
        _rows.Add(newRow);

        return newRow;
    }

    public UpgradeIconView GetNextIcon(Transform parent)
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

    public GameObject GetNextPlaceHolder(Transform parent)
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
