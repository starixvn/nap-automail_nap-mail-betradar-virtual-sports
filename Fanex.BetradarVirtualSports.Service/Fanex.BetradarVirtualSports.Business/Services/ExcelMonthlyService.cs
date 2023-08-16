using Fanex.BetradarVirtualSports.Business.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.BetradarVirtualSports.Business.Services;

public interface ExcelMonthlyService
{
    string Export(
        (IEnumerable<GrossWinningMonthly> firstResultSet, IEnumerable<GrossWinningMonthly> secondResultSet) data);
}

public class ExcelMonthlyServiceImp : ExcelMonthlyService
{
    private readonly DateRangeService dateRangeService;

    public ExcelMonthlyServiceImp(DateRangeService dateRangeService)
    {
        this.dateRangeService = dateRangeService;
    }

    public string Export(
        (IEnumerable<GrossWinningMonthly> firstResultSet, IEnumerable<GrossWinningMonthly> secondResultSet) data)
    {
        using var excelPackage = new ExcelPackage(new FileInfo("Templates/Monthly-Template.xlsx"));
        using var excelWorksheet = excelPackage.Workbook.Worksheets["BetradarVS_Monthly"];
        using var excelTotal = excelPackage.Workbook.Worksheets["Total"];

        excelWorksheet.Cells["A4"].LoadFromCollection(data.Item1, false);

        FormatHeader(excelWorksheet);
        FormatContent(excelWorksheet);
        FormatTotal(excelWorksheet, excelTotal, data.Item2.ToList());

        return Save(excelPackage);
    }

    private void FormatHeader(ExcelWorksheet excelWorksheet)
    {
        var date = dateRangeService.DateRange();
        excelWorksheet.Cells["A2"].Value = $"{date.FromDate:MM/dd/yyyy} - {date.ToDate:MM/dd/yyyy}";
    }

    private static void FormatContent(ExcelWorksheet excelWorksheet)
    {
        const int StartRowIndex = 4;
        var endRowIndex = excelWorksheet.Dimension.End.Row;
        const int StartColumnIndex = 1;
        var endColumnIndex = excelWorksheet.Dimension.End.Column + 1;
        for (var columnIndex = StartColumnIndex; columnIndex < endColumnIndex; columnIndex++)
        {
            excelWorksheet.Cells[StartRowIndex, columnIndex, endRowIndex, columnIndex].StyleID = excelWorksheet.Cells[StartRowIndex, columnIndex].StyleID;
        }

        excelWorksheet.Columns.BestFit = true;
    }

    private static void FormatTotal(ExcelWorksheet excelWorksheet, ExcelWorksheet excelTotal, List<GrossWinningMonthly> dataTotal)
    {
        var startRowIndex = excelWorksheet.Dimension.End.Row + 1;
        const int StartColumnIndex = 1;
        var endColumnIndex = excelWorksheet.Dimension.End.Column + 1;
        excelWorksheet.Cells[$"A{startRowIndex}"].LoadFromCollection(dataTotal, false);
        var endRowIndex = excelWorksheet.Dimension.End.Row;
        for (var columnIndex = StartColumnIndex; columnIndex < endColumnIndex; columnIndex++)
        {
            excelWorksheet.Cells[startRowIndex, columnIndex, endRowIndex, columnIndex].StyleID = excelTotal.Cells[4, columnIndex].StyleID;
        }

        var endDateRowIndex = startRowIndex;
        dataTotal.GroupBy(g => g.Date)
            .ToList()
            .ForEach(date =>
            {
                var dateCount = date.Count();
                var startDateRow = endDateRowIndex;
                var endDateRow = startDateRow + dateCount - 1;
                endDateRowIndex = endDateRow + 1;
                excelWorksheet.Cells[startDateRow, 1, endDateRow, 3].Merge = true;
                excelWorksheet.Cells[startDateRow, 1, endDateRow, 3].Value = "Total In";
            });
    }

    private string Save(ExcelPackage excelPackage)
    {
        excelPackage.Workbook.Worksheets.Delete("Total");
        var date = dateRangeService.DateRange();
        var fileName = $"Betradar_Virtual_Sports_Monthly_Report_{date.FromDate:MMddyyyy}_{date.ToDate:MMddyyyy}.xlsx";
        var path = $@"{AppConfigService.Config.Path}/{DateTime.Now:yyyy}";
        Directory.CreateDirectory(path);
        var fullPath = $@"{path}/{fileName}";
        excelPackage.SaveAs(new FileInfo(fullPath), AppConfigService.Config.Password);
        return fullPath;
    }
}