using System;

namespace Fanex.BetradarVirtualSports.Business.Services;

public interface DateRange
{
    DateTime FromDate { get; }

    DateTime ToDate { get; }

    DateTime RptDate { get; }
}