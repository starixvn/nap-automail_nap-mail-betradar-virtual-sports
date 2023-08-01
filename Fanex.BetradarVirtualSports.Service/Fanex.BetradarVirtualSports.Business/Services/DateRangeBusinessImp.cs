using System;

namespace Fanex.BetradarVirtualSports.Business.Services;

public class DateRangeBusinessImp : DateRange
{
    private const int MaxDay = 31;
    private const int MaxMonth = 12;

    public DateTime FromDate
    {
        get
        {
            var date = DateTime.Now.AddDays(-1);
            if (date.Day == MaxDay && date.Month == MaxMonth)
            {
                return Convert.ToDateTime($"{date:yyyy-MM-01}");
            }

            return Convert.ToDateTime($"{date.AddMonths(-1):yyyy-MM-01}");
        }
    }

    public DateTime ToDate
    {
        get
        {
            var date = DateTime.Now.AddDays(-1);
            if (date.Day == MaxDay && date.Month == MaxMonth)
            {
                return Convert.ToDateTime($"{date:yyyy-MM-dd}");
            }

            date = Convert.ToDateTime($"{date:yyyy-MM-dd}");
            var numOfDays = date.Day;
            return date.AddDays(-numOfDays);
        }
    }

    public DateTime RptDate
    {
        get
        {
            var date = $"{DateTime.Now.AddDays(-1):yyyy/MM/dd}";
            return Convert.ToDateTime(date);
        }
    }
}