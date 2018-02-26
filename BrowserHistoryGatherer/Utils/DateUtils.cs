using System;
using System.Globalization;

namespace BrowserHistoryGatherer.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateUtils
    {
        public static bool TryParsePlistToLocal(string dateString, out DateTime result)
        {
            result = DateTime.Parse("1/1/1");

            if (!double.TryParse(dateString, NumberStyles.Any, CultureInfo.InvariantCulture, out double msSince))
                return false;

            result = result.AddSeconds(msSince).ToLocalTime();
            return true;
        }
    }
}