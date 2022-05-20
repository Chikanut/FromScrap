using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UpgradesInfoPanelPiramid : UpgradesInfoPanelBase
{
    [Header("Settings")]
    [SerializeField] private int _maxItemsInRow = 2;
    
    public override void Visualise(List<UpgradeInfo> info)
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
                    var icon = GetNextIcon(row);
                    
                    UpdateIconInfo(icon, info[0]); 
                    
                    info.RemoveAt(0);
                }
                else
                    GetNextPlaceHolder(row);
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
}
