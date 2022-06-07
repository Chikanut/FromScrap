using FromScrap.Tools;
using TMPro;
using UnityEngine;

public class StatisticPanelView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _value;
    [SerializeField] private TextMeshProUGUI _record;

    public void SetInfo(StatisticType type, int value, int record)
    {
        _value.text = UI_Extentions.GetValue(value, type);
        _record.text = UI_Extentions.GetValue(record, type);
    }
}
