using Fanex.BetradarVirtualSports.Business.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.BetradarVirtualSports.Business.Services;

public interface ExcelDailyService
{
    string Export((IEnumerable<GrossWinningDaily> firstResultSet, IEnumerable<GrossWinningDaily> secondResultSet) data);
}

public class ExcelDailyServiceImp : ExcelDailyService
{
    private readonly DateRangeService dateRangeService;

    public ExcelDailyServiceImp(DateRangeService dateRangeService)
    {
        this.dateRangeService = dateRangeService;
    }

    public string Export(
        (IEnumerable<GrossWinningDaily> firstResultSet, IEnumerable<GrossWinningDaily> secondResultSet) data)
    {
        using var excelPackage = new ExcelPackage(new FileInfo("Templates/Daily-Template.xlsx"));
        using var excelWorksheet = excelPackage.Workbook.Worksheets["BetradarVS_Daily"];
        using var excelTotal = excelPackage.Workbook.Worksheets["Total"];

        excelWorksheet.Cells["A4"].LoadFromCollection(data.Item1, false);

        FormatHeader(excelWorksheet);
        FormatContent(excelWorksheet, data.Item1);
        FormatTotal(excelWorksheet, excelTotal, data.Item2.ToList());

        return Save(excelPackage);
    }

    private void FormatHeader(ExcelWorksheet excelWorksheet)
    {
        var date = dateRangeService.DateRange();
        excelWorksheet.Cells["A2"].Value = $"{date.RptDate:MM/dd/yyyy}";
    }

    private static void FormatContent(ExcelWorksheet excelWorksheet, IEnumerable<GrossWinningDaily> dataContent)
    {
        const int one = 1;
        const int two = 2;
        const int StartRowIndex = 4;
        var endRowIndex = excelWorksheet.Dimension.End.Row;
        const int StartColumnIndex = 1;
        var endColumnIndex = excelWorksheet.Dimension.End.Column + one;
        for (var columnIndex = StartColumnIndex; columnIndex < endColumnIndex; columnIndex++)
        {
            excelWorksheet.Cells[StartRowIndex, columnIndex, endRowIndex, columnIndex].StyleID = excelWorksheet.Cells[StartRowIndex, columnIndex].StyleID;
        }

        excelWorksheet.Columns.BestFit = true;
        var endDateRowIndex = StartRowIndex;
        var endBusinessRowIndex = StartRowIndex;
        dataContent.GroupBy(g => g.Date)
            .ToList()
            .ForEach(date =>
            {
                var dateCount = date.Count();
                var startDateRow = endDateRowIndex;
                var endDateRow = startDateRow + dateCount - one;
                endDateRowIndex = endDateRow + one;
                excelWorksheet.Cells[startDateRow, one, endDateRow, one].Merge = true;

                date.GroupBy(g => g.Business)
                    .ToList()
                    .ForEach(business =>
                    {
                        var businessCount = business.Count();
                        var startBusinessRow = endBusinessRowIndex;
                        var endBusinessRow = startBusinessRow + businessCount - one;
                        endBusinessRowIndex = endBusinessRow + one;
                        excelWorksheet.Cells[startBusinessRow, two, endBusinessRow, two].Merge = true;
                    });
            });
    }

    private static void FormatTotal(ExcelWorksheet excelWorksheet, ExcelWorksheet excelTotal, List<GrossWinningDaily> dataTotal)
    {
        const int one = 1;
        const int two = 2;
        const int four = 4;
        var startRowIndex = excelWorksheet.Dimension.End.Row + one;
        const int StartColumnIndex = 1;
        var endColumnIndex = excelWorksheet.Dimension.End.Column + one;
        excelWorksheet.Cells[$"A{startRowIndex}"].LoadFromCollection(dataTotal, false);
        var endRowIndex = excelWorksheet.Dimension.End.Row;
        for (var columnIndex = StartColumnIndex; columnIndex < endColumnIndex; columnIndex++)
        {
            excelWorksheet.Cells[startRowIndex, columnIndex, endRowIndex, columnIndex].StyleID = excelTotal.Cells[four, columnIndex].StyleID;
        }

        var endDateRowIndex = startRowIndex;
        dataTotal.GroupBy(g => g.Date)
            .ToList()
            .ForEach(date =>
            {
                var dateCount = date.Count();
                var startDateRow = endDateRowIndex;
                var endDateRow = startDateRow + dateCount - one;
                endDateRowIndex = endDateRow + one;
                excelWorksheet.Cells[startDateRow, one, endDateRow, two].Merge = true;
                excelWorksheet.Cells[startDateRow, one, endDateRow, two].Value = "Total In";
            });
    }

    private string Save(ExcelPackage excelPackage)
    {
        excelPackage.Workbook.Worksheets.Delete("Total");
        var date = dateRangeService.DateRange();
        var fileName = $"Betradar_Virtual_Sports_Daily_Report_{date.RptDate:MMddyyyy}.xlsx";
        var path = $@"{AppConfigService.Config.Path}/{DateTime.Now:yyyyMMdd}";
        Directory.CreateDirectory(path);
        var fullPath = $@"{path}/{fileName}";
        excelPackage.SaveAs(new FileInfo(fullPath), AppConfigService.Config.Password);
        return fullPath;
    }
}