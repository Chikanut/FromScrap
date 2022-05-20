using System.Collections.Generic;

public class UpgradesInfoPanelChess : UpgradesInfoPanelBase
{
    public override void Visualise(List<UpgradeInfo> info)
    {
        for (int i = 0; i < info.Count; i++)
        {
            var row = GetNextRow();

            if (i % 2 != 0)
                GetNextPlaceHolder(row);

            var icon = GetNextIcon(row);
            UpdateIconInfo(icon, info[i]);
        }
    }
}
