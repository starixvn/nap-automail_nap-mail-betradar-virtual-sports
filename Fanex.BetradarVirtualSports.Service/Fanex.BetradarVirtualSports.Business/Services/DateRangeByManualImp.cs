using System;

namespace Fanex.BetradarVirtualSports.Business.Services;

public class DateRangeByManualImp : DateRange
{
    public DateTime FromDate => AppConfigService.Config.FromDate;

    public DateTime ToDate => AppConfigService.Config.ToDate;

    public DateTime RptDate => AppConfigService.Config.RptDate;
}