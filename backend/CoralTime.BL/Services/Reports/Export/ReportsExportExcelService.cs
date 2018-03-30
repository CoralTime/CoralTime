using System;
using CoralTime.Common.Constants;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.ViewModels.Reports;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        //private XSSFFont DefaultFont { get; set; }
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T) formatter.Deserialize(ms);
            }
        }

        [Serializable]
        private class SetupStyle
        {

        }

        private int RowIndex { get; set; } = 0;

        private const int MaxCountOfColumns = 10;

        private int CountColumnsOfFirstSize { get; set; } = 0; // Date, Client, Project, Member.
        private int CountColumnsOfSecondSize { get; set; } = 0; // Start, Finish, Act Est
        private int CountColumnsOfThirdSize { get; set; } = 0; // Notes

        private const short WideColumnsFirstSize = 7000; 
        private const short WideColumnsSecondSize = 5000;
        private const short WideColumnsThirdSize = 10000;

        private XSSFCellStyle CreateDefaultStyle(XSSFWorkbook workbook, ISheet sheet)
        {
            var defaultStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            defaultStyle.SetFont(CreateDefaultFont(workbook));
            defaultStyle.WrapText = true;
            defaultStyle.Alignment = HorizontalAlignment.Left;
            defaultStyle.VerticalAlignment = VerticalAlignment.Center;
            //defaultStyle.BorderBottom = BorderStyle.Thin;
            //defaultStyle.BorderTop = BorderStyle.Thin;
            //defaultStyle.BorderLeft = BorderStyle.Thin;
            //defaultStyle.BorderRight = BorderStyle.Thin;

            for (var i = 0; i < MaxCountOfColumns; i++)
            {
                sheet.SetDefaultColumnStyle(i, defaultStyle);
            }

            return defaultStyle;
        }

        private XSSFCellStyle CreateTimeFormatStyle(XSSFWorkbook workbook, IFont font)
        {
            var timeStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            timeStyle.SetFont(font ?? CreateDefaultFont(workbook));
            timeStyle.WrapText = true;
            timeStyle.Alignment = HorizontalAlignment.Left;
            timeStyle.VerticalAlignment = VerticalAlignment.Center;
            timeStyle.DataFormat = workbook.CreateDataFormat().GetFormat("hh:mm");

            return timeStyle;
        }

        private XSSFFont CreateDefaultFont(XSSFWorkbook workbook)
        {
            var font = (XSSFFont)workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.FontName = "Roboto Regular";

            return font;
        }

        private XSSFFont CreateFontColorToAqua(XSSFWorkbook workbook)
        {
            var font = CreateDefaultFont(workbook);
            
            font.SetColor(new XSSFColor(new byte[] { 0, 100, 230 })); //font.Color = IndexedColors.Aqua.Index;

            return font;
        }

        private XSSFFont CreateFontToBold(XSSFWorkbook workbook)
        {
            var font = CreateDefaultFont(workbook);

            font.IsBold = true;

            return font;
        }

        private static void SetFontToCell(ICell cell, XSSFFont font)
        {
            cell.CellStyle.SetFont(font);
        }

        private byte[] CreateFileExcel(ReportTotalView reportTotalView)
        {
            using (var memoryStream = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                
                var sheet = workbook.CreateSheet("Group By " + GetDescriptionGroupById(reportTotalView.GroupByTypeId));

                var defaultStyle = CreateDefaultStyle(workbook, sheet);

                // PeriodCell
                CreateRowOfPeriodDates(reportTotalView, sheet);

                ++RowIndex;

                foreach (var groupedItems in reportTotalView.GroupedItems)
                {
                    ++RowIndex;

                    // GROUP BY TYPE
                    CreateRowOfGroupByType(sheet, groupedItems, workbook);

                    ++RowIndex;

                    // LIST OF HEADERS
                    CreateRowsOfListOfHeaders(groupedItems, sheet, workbook);

                    ++RowIndex;

                    // List of Items
                    foreach (var groupedItem in groupedItems.Items)
                    {
                        CreateRowOfItems(reportTotalView, groupedItems, groupedItem, sheet, workbook);

                        ++RowIndex;
                    }

                    // TOTAL FOR
                    CreateRowOfTotalFor(sheet, groupedItems, workbook);

                    ++RowIndex;
                }

                ++RowIndex;

                // TOTAL 
                CreateRowOfTotal(reportTotalView, sheet, workbook);

                workbook.Write(memoryStream);

                return memoryStream.ToArray();
            }
        }

        private void CreateRowOfPeriodDates(ReportTotalView reportTotalView, ISheet sheet)
        {
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 3));

            var rowPeriod = sheet.CreateRow(RowIndex);
            rowPeriod.CreateCell(0).SetCellValue(reportTotalView.PeriodCell.DisplayNamePeriod + reportTotalView.PeriodCell.DisplayNamePeriodValue);
        }

        private void CreateRowOfGroupByType(ISheet sheet, ReportTotalForGroupTypeView groupedItems, XSSFWorkbook workbook)
        {
            var rowGroupByType = sheet.CreateRow(RowIndex);
            var valueGroupByType = groupedItems.GroupByType.GroupByTypeDisplayName?.ToUpper() + groupedItems.GroupByType.GroupByTypeDisplayNameValue?.ToUpper();

            var cell = rowGroupByType.CreateCell(0);
            cell.SetCellValue(valueGroupByType);

            SetFontToCell(cell, CreateFontColorToAqua(workbook));
        }

        private void CreateRowsOfListOfHeaders(ReportTotalForGroupTypeView groupedItems, ISheet sheet, XSSFWorkbook workbook)
        {
            var listOfHeaders = new List<string>();
            if (groupedItems.DisplayNames.DisplayNameDate != null)
            {
                ++CountColumnsOfFirstSize;
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameDate);
            }

            if (groupedItems.DisplayNames.DisplayNameClient != null)
            {
                ++CountColumnsOfFirstSize;
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameClient);
            }

            if (groupedItems.DisplayNames.DisplayNameProject != null)
            {
                ++CountColumnsOfFirstSize;
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameProject);
            }

            if (groupedItems.DisplayNames.DisplayNameMember != null)
            {
                ++CountColumnsOfFirstSize;
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameMember);
            }

            listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTask);
            if (groupedItems.DisplayNames.DisplayNameTimeFrom != null)
            {
                ++CountColumnsOfFirstSize;
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTimeFrom);
            }

            if (groupedItems.DisplayNames.DisplayNameTimeTo != null)
            {
                ++CountColumnsOfSecondSize;
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTimeTo);
            }

            listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTimeActual);
            if (groupedItems.DisplayNames.DisplayNameTimeEstimated != null)
            {
                ++CountColumnsOfSecondSize;
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTimeEstimated);
            }

            if (groupedItems.DisplayNames.DisplayNameNotes != null)
            {
                ++CountColumnsOfThirdSize;
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameNotes);
            }

            var rowListOfHeaders = sheet.CreateRow(RowIndex);
            for (var i = 0; i < listOfHeaders.Count; i++)
            {
                var cellHeader = rowListOfHeaders.CreateCell(i);
                cellHeader.SetCellValue(listOfHeaders[i]);

                SetFontToCell(cellHeader, CreateFontToBold(workbook));
            }

            // Set width for each type of column
            for (var i = 0; i < CountColumnsOfFirstSize; i++)
            {
                sheet.SetColumnWidth(i, WideColumnsFirstSize);
            }

            for (var i = 0; i < CountColumnsOfSecondSize; i++)
            {
                sheet.SetColumnWidth(i, WideColumnsSecondSize);
            }

            for (var i = 0; i < CountColumnsOfThirdSize; i++)
            {
                sheet.SetColumnWidth(i, WideColumnsThirdSize);
            }
        }

        private void CreateRowOfItems(ReportTotalView reportTotalView, ReportTotalForGroupTypeView groupedItems, ReportItemsView groupedItem, ISheet sheet, XSSFWorkbook workbook)
        {
            var listOfValues = new List<string>();

            if (groupedItems.DisplayNames.DisplayNameDate != null)
            {
                listOfValues.Add(ConvertModelToView.UpdateDateFormat(groupedItem.Date, reportTotalView.DateFormatId));
            }

            if (groupedItems.DisplayNames.DisplayNameClient != null)
            {
                listOfValues.Add(groupedItem.ClientName);
            }

            if (groupedItems.DisplayNames.DisplayNameProject != null)
            {
                listOfValues.Add(groupedItem.ProjectName);
            }

            if (groupedItems.DisplayNames.DisplayNameMember != null)
            {
                listOfValues.Add(groupedItem.MemberName);
            }

            listOfValues.Add(groupedItem.TaskName);
            if (groupedItems.DisplayNames.DisplayNameTimeFrom != null)
            {
                listOfValues.Add(groupedItem.TimeValues.TimeFrom == 0
                    ? null
                    : ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeFrom.ToString()));
            }

            if (groupedItems.DisplayNames.DisplayNameTimeTo != null)
            {
                listOfValues.Add(groupedItem.TimeValues.TimeTo == 0
                    ? null
                    : ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeTo.ToString()));
            }

            listOfValues.Add(ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeActual.ToString()));
            if (groupedItems.DisplayNames.DisplayNameTimeEstimated != null)
            {
                listOfValues.Add(groupedItem.TimeValues.TimeEstimated == 0
                    ? null
                    : ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeEstimated.ToString()));
            }

            if (groupedItems.DisplayNames.DisplayNameNotes != null)
            {
                listOfValues.Add(groupedItem.Notes);
            }

            var rowListOfValues = sheet.CreateRow(RowIndex);
            for (var i = 0; i < listOfValues.Count; i++)
            {
                rowListOfValues.CreateCell(i).SetCellValue(listOfValues[i]);
            }
        }

        private void CreateRowOfTotalFor(ISheet sheet, ReportTotalForGroupTypeView groupedItems, XSSFWorkbook workbook)
        {
            var indexCellTotalFor = 0;

            var rowTotalFor = sheet.CreateRow(RowIndex);
            var cellTotalFor = rowTotalFor.CreateCell(indexCellTotalFor);
            cellTotalFor.SetCellValue(groupedItems.TimeTotalFor.DisplayNameTimeActualTotalFor.ToUpper() + groupedItems.GroupByType.GroupByTypeDisplayNameValue?.ToUpper());
            SetFontToCell(cellTotalFor, CreateFontColorToAqua(workbook));

            if (groupedItems.DisplayNames.DisplayNameDate != null || groupedItems.GroupByTypeId == (int)Constants.ReportsGroupBy.Date)
            {
                rowTotalFor.CreateCell(++indexCellTotalFor).SetCellValue(string.Empty);
            }

            rowTotalFor.CreateCell(++indexCellTotalFor).SetCellValue(string.Empty);
            rowTotalFor.CreateCell(++indexCellTotalFor).SetCellValue(string.Empty);
            if (groupedItems.DisplayNames.DisplayNameTimeFrom != null)
            {
                rowTotalFor.CreateCell(++indexCellTotalFor).SetCellValue(string.Empty);
            }

            if (groupedItems.DisplayNames.DisplayNameTimeTo != null)
            {
                rowTotalFor.CreateCell(++indexCellTotalFor).SetCellValue(string.Empty);
            }

            var actTotalForValues = ConvertModelToView.UpdateTimeFormatForValue(groupedItems.TimeTotalFor.TimeActualTotalFor.ToString());
            var cellTotalForActTime = rowTotalFor.CreateCell(++indexCellTotalFor);
            cellTotalForActTime.SetCellValue(actTotalForValues);
            cellTotalForActTime.CellStyle = CreateTimeFormatStyle(workbook, CreateFontToBold(workbook));

            if (groupedItems.DisplayNames.DisplayNameTimeEstimated != null)
            {
                var totalForEst = groupedItems.TimeTotalFor.TimeEstimatedTotalFor == 0
                    ? null
                    : ConvertModelToView.UpdateTimeFormatForValue(groupedItems.TimeTotalFor.TimeEstimatedTotalFor.ToString());

                var cellTotalForEstTime = rowTotalFor.CreateCell(++indexCellTotalFor);
                cellTotalForEstTime.SetCellValue(totalForEst);
                cellTotalForEstTime.CellStyle = CreateTimeFormatStyle(workbook, CreateFontToBold(workbook));
            }

            if (groupedItems.DisplayNames.DisplayNameNotes != null)
            {
                rowTotalFor.CreateCell(++indexCellTotalFor).SetCellValue(string.Empty);
            }
        }

        private void CreateRowOfTotal(ReportTotalView reportTotalView, ISheet sheet, XSSFWorkbook workbook)
        {
            var indexCellTotal = 0;

            var rowTotal = sheet.CreateRow(RowIndex);
            var cellTotal = rowTotal.CreateCell(indexCellTotal);
            cellTotal.SetCellValue(reportTotalView.TimeTotal.DisplayNameTimeActualTotal);

            SetFontToCell(cellTotal, CreateFontColorToAqua(workbook));

            if (reportTotalView.DisplayNames.DisplayNameDate != null || reportTotalView.GroupByTypeId == (int) Constants.ReportsGroupBy.Date)
            {
                rowTotal.CreateCell(++indexCellTotal).SetCellValue(string.Empty);
            }

            rowTotal.CreateCell(++indexCellTotal).SetCellValue(string.Empty);
            rowTotal.CreateCell(++indexCellTotal).SetCellValue(string.Empty);
            if (reportTotalView.DisplayNames.DisplayNameTimeFrom != null)
            {
                rowTotal.CreateCell(++indexCellTotal).SetCellValue(string.Empty);
            }

            if (reportTotalView.DisplayNames.DisplayNameTimeTo != null)
            {
                rowTotal.CreateCell(++indexCellTotal).SetCellValue(string.Empty);
            }

            var actualTotalValue = ConvertModelToView.UpdateTimeFormatForValue(reportTotalView.TimeTotal.TimeActualTotal.ToString());
            var cellTotalAct = rowTotal.CreateCell(++indexCellTotal);
            cellTotalAct.SetCellValue(actualTotalValue);
            cellTotalAct.CellStyle = CreateTimeFormatStyle(workbook, CreateFontToBold(workbook));

            if (reportTotalView.DisplayNames.DisplayNameTimeEstimated != null)
            {
                var totalEst = reportTotalView.TimeTotal.TimeEstimatedTotal == 0
                    ? null
                    : ConvertModelToView.UpdateTimeFormatForValue(reportTotalView.TimeTotal.TimeEstimatedTotal.ToString());

                var cellTotalEst = rowTotal.CreateCell(++indexCellTotal);
                cellTotalEst.SetCellValue(totalEst);
                cellTotalEst.CellStyle = CreateTimeFormatStyle(workbook, CreateFontToBold(workbook));
            }

            if (reportTotalView.DisplayNames.DisplayNameNotes != null)
            {
                rowTotal.CreateCell(++indexCellTotal).SetCellValue(string.Empty);
            }
        }

        private string GetDescriptionGroupById(int? groupById)
        {
            switch (groupById)
            {
                case (int) Constants.ReportsGroupBy.Project:
                {
                    return Constants.ReportsGroupBy.Project.ToString();
                }

                case (int) Constants.ReportsGroupBy.Member:
                {
                    return Constants.ReportsGroupBy.Member.ToString();
                }

                case (int) Constants.ReportsGroupBy.Date:
                {
                    return Constants.ReportsGroupBy.Date.ToString();
                }

                case (int) Constants.ReportsGroupBy.Client:
                {
                    return Constants.ReportsGroupBy.Client.ToString();
                }

                default:
                {
                    return Constants.ReportsGroupBy.UnknownGrouping.ToString();
                }
            }
        }
    }
}