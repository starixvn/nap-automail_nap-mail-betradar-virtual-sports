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
        (
            IEnumerable<GrossWinningMonthly> firstResultSet,
            IEnumerable<GrossWinningMonthly> secondResultSet) data,
        Product product
    );
}

public class ExcelMonthlyServiceImp(DateRangeService dateRangeService) : ExcelMonthlyService
{
    public string Export(
        (IEnumerable<GrossWinningMonthly> firstResultSet, IEnumerable<GrossWinningMonthly> secondResultSet) data,
        Product product)
    {
        var sheetsNames = new Dictionary<Product, string>
        {
            { Product.VS2, "BetradarVS_Monthly" },
            { Product.VS3, "VS3_Monthly" },
            { Product.VS4, "VS4_Monthly" },
            { Product.VS5, "VS5_Monthly" },
            { Product.VS7, "VS7_Monthly" },
        };

        var sheetName = sheetsNames[product];
        using var excelPackage = new ExcelPackage(new FileInfo($"Templates/Monthly-{product}-Template.xlsx"));
        using var excelWorksheet = excelPackage.Workbook.Worksheets[sheetName];
        using var excelTotal = excelPackage.Workbook.Worksheets["Total"];

        excelWorksheet.Cells["A4"].LoadFromCollection(data.firstResultSet, false);
        FormatHeader(excelWorksheet);

        if (data.firstResultSet.Any())
        {
            FormatContent(excelWorksheet);
            FormatTotal(excelWorksheet, excelTotal, data.secondResultSet.ToList());
        }
        else
        {
            excelWorksheet.Cells["A4:J4"].Value = "There is no data displayed";
            excelWorksheet.Cells["A4:J4"].Merge = true;
        }

        return Save(excelPackage, product);
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
            excelWorksheet.Cells[StartRowIndex, columnIndex, endRowIndex, columnIndex].StyleID =
                excelWorksheet.Cells[StartRowIndex, columnIndex].StyleID;
        }

        excelWorksheet.Columns.BestFit = true;
    }

    private static void FormatTotal(ExcelWorksheet excelWorksheet, ExcelWorksheet excelTotal,
        List<GrossWinningMonthly> dataTotal)
    {
        const int one = 1;
        const int three = 3;
        const int four = 4;
        var startRowIndex = excelWorksheet.Dimension.End.Row + one;
        const int startColumnIndex = one;
        var endColumnIndex = excelWorksheet.Dimension.End.Column + one;
        excelWorksheet.Cells[$"A{startRowIndex}"].LoadFromCollection(dataTotal, false);
        var endRowIndex = excelWorksheet.Dimension.End.Row;
        for (var columnIndex = startColumnIndex; columnIndex < endColumnIndex; columnIndex++)
        {
            excelWorksheet.Cells[startRowIndex, columnIndex, endRowIndex, columnIndex].StyleID =
                excelTotal.Cells[four, columnIndex].StyleID;
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
                excelWorksheet.Cells[startDateRow, one, endDateRow, three].Merge = true;
                excelWorksheet.Cells[startDateRow, one, endDateRow, three].Value = "Total In";
            });
    }

    private string Save(ExcelPackage excelPackage, Product product)
    {
        var fileNames = new Dictionary<Product, string>
        {
            { Product.VS2, "Betradar_Virtual_Sports_Monthly_Report" },
            { Product.VS3, "Virtual_Sports3_Monthly_Report" },
            { Product.VS4, "Virtual_Sports4_Monthly_Report" },
            { Product.VS5, "Virtual_Sports5_Monthly_Report" },
            { Product.VS7, "Virtual_Sports7_Monthly_Report" },
        };

        excelPackage.Workbook.Worksheets.Delete("Total");
        var date = dateRangeService.DateRange();
        var fileName = $"{fileNames[product]}_{date.FromDate:MMddyyyy}_{date.ToDate:MMddyyyy}.xlsx";
        var path = $@"{AppConfigService.Config.Path}/{DateTimeOffset.Now:yyyyMMdd}";
        Directory.CreateDirectory(path);
        var fullPath = $@"{path}/{fileName}";

        if (product == Product.VS4)
        {
            excelPackage.SaveAs(new FileInfo(fullPath));
        }
        else
        {
            excelPackage.SaveAs(new FileInfo(fullPath), AppConfigService.Config.Password);
        }

        return fullPath;
    }
}