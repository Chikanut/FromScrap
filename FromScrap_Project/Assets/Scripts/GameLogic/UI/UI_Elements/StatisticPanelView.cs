using System;
using TMPro;
using UnityEngine;

public class StatisticPanelView : MonoBehaviour
{
    public enum  StatisticType
    {
        count,
        time
    }

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _value;
    [SerializeField] private TextMeshProUGUI _record;

    public void SetInfo(StatisticType type, int value, int record)
    {
        _value.text = GetValue(value, type);
        _record.text = GetValue(record, type);
    }

    string GetValue(int value, StatisticType type)
    {
        switch (type)
        {
            case StatisticType.count:
                return value.ToString("N0");
            case StatisticType.time:
                var time = TimeSpan.FromSeconds(value);
                var str = time.ToString(@"mm\:ss");
                return str;
            default:
                return value.ToString("N0");
        }
    }
}
