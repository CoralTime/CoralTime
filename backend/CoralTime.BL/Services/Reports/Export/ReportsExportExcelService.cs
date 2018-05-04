using CoralTime.Common.Constants;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.ViewModels.Reports;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        private int RowIndex { get; set; } = 0;

        private const short WideColumnsFirstSize = 5000; 
        private const short WideColumnsSecondSize = 3000;
        private const short WideColumnsThirdSize = 10000;

        private XSSFCellStyle CreateDefStyle(XSSFWorkbook workbook)
        {
            var defStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            defStyle.SetFont(CreateDefaultFont(workbook));

            defStyle.WrapText = true;
            defStyle.Alignment = HorizontalAlignment.Left;
            defStyle.VerticalAlignment = VerticalAlignment.Center;

            return defStyle;
        }

        private XSSFCellStyle CreateGroupByTotalStyle(XSSFWorkbook workbook)
        {
            var groupByTotalStyle = CreateDefStyle(workbook);

            groupByTotalStyle.SetFont(CreateFontColorAqua(workbook));

            return groupByTotalStyle;
        }

        private XSSFCellStyle CreateHeadersStyle(XSSFWorkbook workbook)
        {
            var headersStyle = CreateDefStyle(workbook);

            headersStyle.SetFont(CreateFontBold(workbook));

            headersStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            headersStyle.FillPattern = FillPattern.SolidForeground;

            return headersStyle;
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

        private XSSFCellStyle CreateTimeTotalFormatStyle(XSSFWorkbook workbook, IFont font)
        {
            var timeStyle = CreateTimeFormatStyle(workbook, font);

            timeStyle.DataFormat = workbook.CreateDataFormat().GetFormat("[h]:mm");

            return timeStyle;
        }

        private XSSFFont CreateDefaultFont(XSSFWorkbook workbook)
        {
            var font = (XSSFFont)workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.FontName = "Roboto Regular";

            return font;
        }

        private XSSFFont CreateFontColorAqua(XSSFWorkbook workbook)
        {
            var font = CreateDefaultFont(workbook);

            var auaColor = new XSSFColor(new byte[] {0, 100, 230});
            font.SetColor(auaColor); 
            
            //font.Color = IndexedColors.Aqua.Index;

            return font;
        }

        private XSSFFont CreateFontBold(XSSFWorkbook workbook)
        {
            var font = CreateDefaultFont(workbook);

            font.IsBold = true;

            return font;
        }

        private byte[] CreateFileExcel(ReportTotalView reportTotalView)
        {
            using (var memoryStream = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                
                var sheet = workbook.CreateSheet("Group By " + GetDescriptionGroupById(reportTotalView.GroupByTypeId));

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

                SetWidthForEachTypeOfColumn(reportTotalView.GroupedItems.FirstOrDefault(), sheet);
                workbook.Write(memoryStream);

                return memoryStream.ToArray();
            }
        }

        private void SetWidthForEachTypeOfColumn(ReportTotalForGroupTypeView groupedItems, ISheet sheet)
        {
            var сountColumnsOfFirstSize  = 0; // Date, Client, Project, Member.
            var сountColumnsOfSecondSize = 0; // Start, Finish, Act, Est
            var сountColumnsOfThirdSize  = 0; // Notes

            //var listOfHeaders = new List<string>();
            if (groupedItems.DisplayNames.DisplayNameDate != null)
            {
                ++сountColumnsOfFirstSize;
            }

            if (groupedItems.DisplayNames.DisplayNameClient != null)
            {
                ++сountColumnsOfFirstSize;
            }

            if (groupedItems.DisplayNames.DisplayNameProject != null)
            {
                ++сountColumnsOfFirstSize;
            }

            if (groupedItems.DisplayNames.DisplayNameMember != null)
            {
                ++сountColumnsOfFirstSize;
            }

            //listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTask);
            if (groupedItems.DisplayNames.DisplayNameTimeFrom != null)
            {
                ++сountColumnsOfSecondSize;
            }

            if (groupedItems.DisplayNames.DisplayNameTimeTo != null)
            {
                ++сountColumnsOfSecondSize;
            }

            //listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTimeActual);
            if (groupedItems.DisplayNames.DisplayNameTimeEstimated != null)
            {
                ++сountColumnsOfSecondSize;
            }

            if (groupedItems.DisplayNames.DisplayNameNotes != null)
            {
                ++сountColumnsOfThirdSize;
            }

            SetWidthForEachTypeOfColumn(sheet, сountColumnsOfFirstSize, сountColumnsOfSecondSize, сountColumnsOfThirdSize);
        }

        private void CreateRowOfPeriodDates(ReportTotalView reportTotalView, ISheet sheet)
        {
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 1));

            var rowPeriod = sheet.CreateRow(RowIndex);
            rowPeriod.CreateCell(0).SetCellValue(reportTotalView.PeriodCell.DisplayNamePeriod + reportTotalView.PeriodCell.DisplayNamePeriodValue);
        }

        private void CreateRowOfGroupByType(ISheet sheet, ReportTotalForGroupTypeView groupedItems, XSSFWorkbook workbook)
        {
            sheet.AddMergedRegion(new CellRangeAddress(RowIndex, RowIndex, 0, 1));

            var rowGroupByType = sheet.CreateRow(RowIndex);
            var valueGroupByType = groupedItems.GroupByType.GroupByTypeDisplayName?.ToUpper() + groupedItems.GroupByType.GroupByTypeDisplayNameValue?.ToUpper();

            var cellGroupByType = rowGroupByType.CreateCell(0);
            cellGroupByType.SetCellValue(valueGroupByType);
            cellGroupByType.CellStyle = CreateGroupByTotalStyle(workbook);
        }

        private void CreateRowsOfListOfHeaders(ReportTotalForGroupTypeView groupedItems, ISheet sheet, XSSFWorkbook workbook)
        {
            var listOfHeaders = new List<string>();

            if (groupedItems.DisplayNames.DisplayNameDate != null)
            {
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameDate);
            }

            if (groupedItems.DisplayNames.DisplayNameClient != null)
            {
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameClient);
            }

            if (groupedItems.DisplayNames.DisplayNameProject != null)
            {
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameProject);
            }

            if (groupedItems.DisplayNames.DisplayNameMember != null)
            {
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameMember);
            }

            listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTask);
            if (groupedItems.DisplayNames.DisplayNameTimeFrom != null)
            {
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTimeFrom);
            }

            if (groupedItems.DisplayNames.DisplayNameTimeTo != null)
            {
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTimeTo);
            }

            listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTimeActual);
            if (groupedItems.DisplayNames.DisplayNameTimeEstimated != null)
            {
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameTimeEstimated);
            }

            if (groupedItems.DisplayNames.DisplayNameNotes != null)
            {
                listOfHeaders.Add(groupedItems.DisplayNames.DisplayNameNotes);
            }

            var rowListOfHeaders = sheet.CreateRow(RowIndex);
            for (var i = 0; i < listOfHeaders.Count; i++)
            {
                var cellHeader = rowListOfHeaders.CreateCell(i);
                cellHeader.SetCellValue(listOfHeaders[i]);
                cellHeader.CellStyle = CreateHeadersStyle(workbook);
            }
        }

        private void CreateRowOfItems(ReportTotalView reportTotalView, ReportTotalForGroupTypeView groupedItems, ReportItemsView groupedItem, ISheet sheet, XSSFWorkbook workbook)
        {
            var rowListOfValues = sheet.CreateRow(RowIndex);
            var cellIndex = 0;

            if (groupedItems.DisplayNames.DisplayNameDate != null)
            {
                var cellValue = ConvertModelToView.UpdateDateFormat(groupedItem.Date, reportTotalView.DateFormatId);

                CreateCellItem(workbook, rowListOfValues, ref cellIndex, cellValue, CreateDefaultFont(workbook));
            }

            if (groupedItems.DisplayNames.DisplayNameClient != null)
            {
                CreateCellItem(workbook, rowListOfValues, ref cellIndex, groupedItem.ClientName, CreateDefaultFont(workbook));
            }

            if (groupedItems.DisplayNames.DisplayNameProject != null)
            {
                CreateCellItem(workbook, rowListOfValues, ref cellIndex, groupedItem.ProjectName, CreateDefaultFont(workbook));
            }

            if (groupedItems.DisplayNames.DisplayNameMember != null)
            {
                CreateCellItem(workbook, rowListOfValues, ref cellIndex, groupedItem.MemberName, CreateDefaultFont(workbook));
            }

            CreateCellItem(workbook, rowListOfValues, ref cellIndex, groupedItem.TaskName, CreateDefaultFont(workbook));
            
            // 1
            if (groupedItems.DisplayNames.DisplayNameTimeFrom != null)
            {
                var cellValue = groupedItem.TimeValues.TimeFrom == 0
                    ? null
                    : ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeFrom.ToString());

                CreateCellItem(workbook, rowListOfValues, ref cellIndex, cellValue, CreateDefaultFont(workbook));
            }
            // 2
            if (groupedItems.DisplayNames.DisplayNameTimeTo != null)
            {
                var cellValue = groupedItem.TimeValues.TimeTo == 0
                    ? null
                    : ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeTo.ToString());

                CreateCellItem(workbook, rowListOfValues, ref cellIndex, cellValue, CreateDefaultFont(workbook));
            }

            // 3
            var cellActValue =  ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeActual.ToString());
            CreateCellItem(workbook, rowListOfValues, ref cellIndex, cellActValue, CreateDefaultFont(workbook));

            // 4 
            if (groupedItems.DisplayNames.DisplayNameTimeEstimated != null)
            {
                var cellValue = groupedItem.TimeValues.TimeEstimated == 0
                    ? null
                    : ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeEstimated.ToString());

                CreateCellItem(workbook, rowListOfValues, ref cellIndex, cellValue, CreateDefaultFont(workbook));
            }

            if (groupedItems.DisplayNames.DisplayNameNotes != null)
            {
                CreateCellItem(workbook, rowListOfValues, ref cellIndex, groupedItem.Notes, CreateDefaultFont(workbook));
            }
        }

        private void CreateCellItem(XSSFWorkbook workbook, IRow rowListOfValues, ref int cellIndex, string cellValue, IFont font)
        {
            var cellListOfValues = rowListOfValues.CreateCell(cellIndex);
            cellListOfValues.SetCellValue(cellValue);
            cellListOfValues.CellStyle = CreateTimeFormatStyle(workbook, font);

            ++cellIndex;
        }

        private void CreateCellItemTotal(XSSFWorkbook workbook, IRow rowListOfValues, ref int cellIndex, string cellValue, IFont font)
        {
            var cellListOfValues = rowListOfValues.CreateCell(cellIndex);
            cellListOfValues.SetCellValue(cellValue);
            cellListOfValues.CellStyle = CreateTimeTotalFormatStyle(workbook, font);

            ++cellIndex;
        }

        private void CreateRowOfTotalFor(ISheet sheet, ReportTotalForGroupTypeView groupedItems, XSSFWorkbook workbook)
        {
            sheet.AddMergedRegion(new CellRangeAddress(RowIndex, RowIndex, 0, 1));

            var rowTotalFor = sheet.CreateRow(RowIndex);
            var cellIndex = 0;

            var cellValueTotalFor = groupedItems.TimeTotalFor.DisplayNameTimeActualTotalFor.ToUpper() + groupedItems.GroupByType.GroupByTypeDisplayNameValue?.ToUpper();
            CreateCellItem(workbook, rowTotalFor, ref cellIndex, cellValueTotalFor, CreateFontColorAqua(workbook));

            if (groupedItems.DisplayNames.DisplayNameDate != null || groupedItems.GroupByTypeId == (int)Constants.ReportsGroupByIds.Date)
            {
                rowTotalFor.CreateCell(++cellIndex).SetCellValue(string.Empty);
            }

            rowTotalFor.CreateCell(++cellIndex).SetCellValue(string.Empty);
            rowTotalFor.CreateCell(++cellIndex).SetCellValue(string.Empty);
            if (groupedItems.DisplayNames.DisplayNameTimeFrom != null)
            {
                rowTotalFor.CreateCell(++cellIndex).SetCellValue(string.Empty);
            }

            if (groupedItems.DisplayNames.DisplayNameTimeTo != null)
            {
                rowTotalFor.CreateCell(++cellIndex).SetCellValue(string.Empty);
            }

            // 1
            var cellValueActTotal = ConvertModelToView.UpdateTimeFormatForValue(groupedItems.TimeTotalFor.TimeActualTotalFor.ToString());
            CreateCellItem(workbook, rowTotalFor, ref cellIndex, cellValueActTotal, CreateFontBold(workbook));

            // 2
            if (groupedItems.DisplayNames.DisplayNameTimeEstimated != null)
            {
                var cellValueEstTotal = groupedItems.TimeTotalFor.TimeEstimatedTotalFor == 0
                    ? null
                    : ConvertModelToView.UpdateTimeFormatForValue(groupedItems.TimeTotalFor.TimeEstimatedTotalFor.ToString());

                CreateCellItem(workbook, rowTotalFor, ref cellIndex, cellValueEstTotal, CreateFontBold(workbook));
            }

            if (groupedItems.DisplayNames.DisplayNameNotes != null)
            {
                rowTotalFor.CreateCell(++cellIndex).SetCellValue(string.Empty);
            }
        }

        private void CreateRowOfTotal(ReportTotalView reportTotalView, ISheet sheet, XSSFWorkbook workbook)
        {
            var rowTotal = sheet.CreateRow(RowIndex);
            var cellIndex = 0;

            var cellValueTotal = reportTotalView.TimeTotal.DisplayNameTimeActualTotal.ToUpper();
            CreateCellItemTotal(workbook, rowTotal, ref cellIndex, cellValueTotal, CreateFontColorAqua(workbook));

            if (reportTotalView.DisplayNames.DisplayNameDate != null || reportTotalView.GroupByTypeId == (int) Constants.ReportsGroupByIds.Date)
            {
                rowTotal.CreateCell(++cellIndex).SetCellValue(string.Empty);
            }

            rowTotal.CreateCell(++cellIndex).SetCellValue(string.Empty);
            rowTotal.CreateCell(++cellIndex).SetCellValue(string.Empty);
            if (reportTotalView.DisplayNames.DisplayNameTimeFrom != null)
            {
                rowTotal.CreateCell(++cellIndex).SetCellValue(string.Empty);
            }

            if (reportTotalView.DisplayNames.DisplayNameTimeTo != null)
            {
                rowTotal.CreateCell(++cellIndex).SetCellValue(string.Empty);
            }

            // 1 
            var cellValueActTotal= ConvertModelToView.UpdateTimeFormatForValue(reportTotalView.TimeTotal.TimeActualTotal.ToString());
            CreateCellItemTotal(workbook, rowTotal, ref cellIndex, cellValueActTotal, CreateFontBold(workbook));
            // 2
            if (reportTotalView.DisplayNames.DisplayNameTimeEstimated != null)
            {
                var cellValueEstTotal = reportTotalView.TimeTotal.TimeEstimatedTotal == 0
                    ? null
                    : ConvertModelToView.UpdateTimeFormatForValue(reportTotalView.TimeTotal.TimeEstimatedTotal.ToString());

                CreateCellItemTotal(workbook, rowTotal, ref cellIndex, cellValueEstTotal, CreateFontBold(workbook));
            }

            if (reportTotalView.DisplayNames.DisplayNameNotes != null)
            {
                rowTotal.CreateCell(++cellIndex).SetCellValue(string.Empty);
            }
        }

        private void SetWidthForEachTypeOfColumn(ISheet sheet, int сountColumnsOfFirstSize, int сountColumnsOfSecondSize, int сountColumnsOfThirdSize)
        {
            // Set width for each type of column
            var startIndexFirtsColumns = 0;
            var endIndexFirtsColumns = startIndexFirtsColumns + (сountColumnsOfFirstSize == 1 ? 0 : сountColumnsOfFirstSize);
            SetColumnWidthByRange(sheet, startIndexFirtsColumns, endIndexFirtsColumns, WideColumnsFirstSize);

            var startIndexSecondColumns = сountColumnsOfFirstSize + 1;
            var endIndexSecondColumns = startIndexSecondColumns + (сountColumnsOfSecondSize == 1 ? 0 : сountColumnsOfSecondSize);
            SetColumnWidthByRange(sheet, startIndexSecondColumns, endIndexSecondColumns, WideColumnsSecondSize);

            var startIndexThirdColumns = endIndexSecondColumns + 1;
            var endIndexThirdColumns = startIndexThirdColumns + (сountColumnsOfThirdSize == 1 ? 0 : сountColumnsOfThirdSize);
            SetColumnWidthByRange(sheet, startIndexThirdColumns, endIndexThirdColumns, WideColumnsThirdSize);
        }

        private void SetColumnWidthByRange(ISheet sheet, int start, int finish, int size)
        {
            for (var i = start; i <= finish; i++)
            {
                sheet.SetColumnWidth(i, size);
            }
        }

        private string GetDescriptionGroupById(int? groupById)
        {
            switch (groupById)
            {
                case (int) Constants.ReportsGroupByIds.Project:
                {
                    return Constants.ReportsGroupByIds.Project.ToString();
                }

                case (int) Constants.ReportsGroupByIds.User:
                {
                    return Constants.ReportsGroupByIds.User.ToString();
                }

                case (int) Constants.ReportsGroupByIds.Date:
                {
                    return Constants.ReportsGroupByIds.Date.ToString();
                }

                case (int) Constants.ReportsGroupByIds.Client:
                {
                    return Constants.ReportsGroupByIds.Client.ToString();
                }

                default:
                {
                    return Constants.ReportsGroupByIds.UnknownGrouping.ToString();
                }
            }
        }
    }
}