using System;
using TMPro;
using UnityEngine;

public class StatisticPanelView : MonoBehaviour
{
    public enum  StatisticType
    {
        count,
        number,
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
                return value.ToString();
            case StatisticType.number:
                return value.ToString("N0");
            case StatisticType.time:
                var time = TimeSpan.FromSeconds(value);
                var str = time.ToString(@"mm\:ss");
                return str;
            default:
                return GetNumber(value);
        }
    }

    string GetNumber(int value)
    {
        var str = "";
        
        var hasMillions = false;
        var hasThousands = false;
        var hasHundreds = false;

        if (value > 1000000)
        {
            str = ((int) (value / 1000000)).ToString("N0") + "M";
            value %= 1000000;
            hasMillions = true;
        }

        if (value > 1000)
        {
            if(hasMillions)
                str += " ";
            
            str += ((int) (value / 1000)).ToString("N0") + "K";
            value %= 1000;
            hasThousands = true;
        }

        if (hasMillions)
            return str;
        
        if (value > 100)
        {
            if (!hasThousands) return str;
            str += " ";
            str += ((int) (value / 100)).ToString("N0") + "H";
        }
        else
        {
            str += value.ToString("N0");
        }

        return str;
    }
}
