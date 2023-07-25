using System.Collections.Generic;

namespace Fanex.BetradarVirtualSports.Business.Services;

public interface DateRangeService
{
    DateRange DateRange();
}

public class DateRangeServiceImp : DateRangeService
{
    private static Dictionary<bool, DateRange> DateRanges => new()
    {
        { false, new DateRangeBusinessImp() },
        { true, new DateRangeByManualImp() }
    };

    public DateRange DateRange()
    {
        return DateRanges[AppConfigService.Config.IsManual];
    }
}