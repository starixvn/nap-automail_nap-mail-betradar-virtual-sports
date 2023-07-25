using Fanex.BetradarVirtualSports.Business.Criteria;
using Fanex.BetradarVirtualSports.Business.Models;
using Fanex.Data.Repository;
using System.Collections.Generic;

namespace Fanex.BetradarVirtualSports.Business.Services;

public interface DataService
{
    (IEnumerable<GrossWinningDaily> firstResultSet, IEnumerable<GrossWinningDaily> secondResultSet) QueryDailyData();

    (IEnumerable<GrossWinningMonthly> firstResultSet, IEnumerable<GrossWinningMonthly> secondResultSet) QueryMonthlyData();

    bool IsDataAvailable();
}

public class DataServiceImp : DataService
{
    private readonly DateRangeService dateService;
    private readonly IDynamicRepository repository;

    public DataServiceImp(DateRangeService dateService, IDynamicRepository repository)
    {
        this.dateService = dateService;
        this.repository = repository;
    }

    public (IEnumerable<GrossWinningDaily> firstResultSet, IEnumerable<GrossWinningDaily> secondResultSet) QueryDailyData()
    {
        var date = dateService.DateRange();
        var criteria = new DailyCriteria { RptDate = date.RptDate };
        return repository.FetchMultiple<GrossWinningDaily, GrossWinningDaily>(criteria);
    }

    public (IEnumerable<GrossWinningMonthly> firstResultSet, IEnumerable<GrossWinningMonthly> secondResultSet) QueryMonthlyData()
    {
        var date = dateService.DateRange();
        var criteria = new MonthlyCriteria { FromDate = date.FromDate, ToDate = date.ToDate };
        var data = repository.FetchMultiple<GrossWinningMonthly, GrossWinningMonthly>(criteria);
        return data;
    }

    public bool IsDataAvailable()
    {
        if (AppConfigService.Config.IsManual || !AppConfigService.Config.IsCheckEnoughData)
        {
            return true;
        }

        return repository.Get<TaskStatusModel>(new DWStatusByCubeCriteria()).DataIsReady;
    }
}