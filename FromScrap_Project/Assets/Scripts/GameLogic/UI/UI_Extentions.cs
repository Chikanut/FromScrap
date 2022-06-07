using System;

namespace FromScrap.Tools
{
    public enum StatisticType
    {
        count,
        time
    }

    public static class UI_Extentions
    {
        public static string GetValue(int value, StatisticType type)
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
}
