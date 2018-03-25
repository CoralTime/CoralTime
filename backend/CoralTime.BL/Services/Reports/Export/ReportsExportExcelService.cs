using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Reports;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        private byte[] CreateFileExcel(DataSet groupingList)
        {
            using (var memoryStream = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();

                //sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                //var rowIndex = 0;
                //var row = sheet.CreateRow(rowIndex);
                //row.Height = 30 * 80;
                //row.CreateCell(0).SetCellValue("this is content");
                //sheet.AutoSizeColumn(0);
                //rowIndex++;

                //var sheet2 = workbook.CreateSheet("Sheet2");
                //var style1 = workbook.CreateCellStyle();
                //style1.FillForegroundColor = HSSFColor.Blue.Index2;
                //style1.FillPattern = FillPattern.SolidForeground;

                //var style2 = workbook.CreateCellStyle();
                //style2.FillForegroundColor = HSSFColor.Yellow.Index2;
                //style2.FillPattern = FillPattern.SolidForeground;

                //var cell2 = sheet2.CreateRow(0).CreateCell(0);
                //cell2.CellStyle = style1;
                //cell2.SetCellValue(0);

                //cell2 = sheet2.CreateRow(1).CreateCell(0);
                //cell2.CellStyle = style2;
                //cell2.SetCellValue(1);

                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}