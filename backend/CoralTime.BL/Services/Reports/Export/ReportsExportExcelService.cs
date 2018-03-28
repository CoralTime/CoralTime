using System.Collections.Generic;
using CoralTime.Common.Constants;
using CoralTime.ViewModels.Reports;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Text;
using CoralTime.DAL.ConvertModelToView;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        private byte[] CreateFileExcel(ReportTotalView reportTotalView)
        {
            using (var memoryStream = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();

                var sheet = workbook.CreateSheet("Group By " + GetDescriptionGroupById(reportTotalView.GroupByTypeId));

                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 3));

                // PeriodCell
                var rowIndex = 0;

                var rowPeriod = sheet.CreateRow(rowIndex);
                rowPeriod.CreateCell(0).SetCellValue(reportTotalView.PeriodCell.DisplayNamePeriod + reportTotalView.PeriodCell.DisplayNamePeriodValue);

                ++rowIndex;

                foreach (var groupedItems in reportTotalView.GroupedItems)
                {
                    ++rowIndex;

                    // GROUP BY TYPE
                    var rowGroupByType = sheet.CreateRow(rowIndex);
                    var valueGroupByType = groupedItems.GroupByType.GroupByTypeDisplayName?.ToUpper() + groupedItems.GroupByType.GroupByTypeDisplayNameValue?.ToUpper();
                    rowGroupByType.CreateCell(0).SetCellValue(valueGroupByType);

                    ++rowIndex;

                    // LIST OF HEADERS
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

                    var rowListOfHeaders = sheet.CreateRow(rowIndex);
                    for (var i = 0; i < listOfHeaders.Count; i++)
                    {
                        rowListOfHeaders.CreateCell(i).SetCellValue(listOfHeaders[i]);
                    }

                    ++rowIndex;

                    // List of Items
                    foreach (var groupedItem in groupedItems.Items)
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
                            listOfValues.Add(groupedItem.TimeValues.TimeFrom == 0 ? null : ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeFrom.ToString()));
                        }
                        if (groupedItems.DisplayNames.DisplayNameTimeTo != null)
                        {
                            listOfValues.Add(groupedItem.TimeValues.TimeTo == 0 ? null : ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeTo.ToString()));
                        }
                        listOfValues.Add(ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeActual.ToString()));
                        if (groupedItems.DisplayNames.DisplayNameTimeEstimated != null)
                        {
                            listOfValues.Add(groupedItem.TimeValues.TimeEstimated == 0 ? null : ConvertModelToView.UpdateTimeFormatForValue(groupedItem.TimeValues.TimeEstimated.ToString()));
                        }
                        if (groupedItems.DisplayNames.DisplayNameNotes != null)
                        {
                            listOfValues.Add(groupedItem.Notes);
                        }

                        var rowListOfValues = sheet.CreateRow(rowIndex);
                        for (var i = 0; i < listOfValues.Count; i++)
                        {
                            rowListOfValues.CreateCell(i).SetCellValue(listOfValues[i]);
                        }

                        ++rowIndex;
                    }

                    // TOTAL FOR
                    var indexCellTotalFor = 0;

                    var rowTotalFor = sheet.CreateRow(rowIndex);
                    rowTotalFor.CreateCell(indexCellTotalFor).SetCellValue(groupedItems.TimeTotalFor.DisplayNameTimeActualTotalFor);

                    if (groupedItems.DisplayNames.DisplayNameDate != null || groupedItems.GroupByTypeId == (int) Constants.ReportsGroupBy.Date)
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
                    rowTotalFor.CreateCell(++indexCellTotalFor).SetCellValue(actTotalForValues);

                    if (groupedItems.DisplayNames.DisplayNameTimeEstimated != null)
                    {
                        var totalForEst = groupedItems.TimeTotalFor.TimeEstimatedTotalFor == 0 ? null : ConvertModelToView.UpdateTimeFormatForValue(groupedItems.TimeTotalFor.TimeEstimatedTotalFor.ToString());
                        rowTotalFor.CreateCell(++indexCellTotalFor).SetCellValue(totalForEst);
                    }

                    if (groupedItems.DisplayNames.DisplayNameNotes != null)
                    {
                        rowTotalFor.CreateCell(++indexCellTotalFor).SetCellValue(string.Empty); 
                    }

                    ++rowIndex;
                }

                ++rowIndex;

                // TOTAL 
                var indexCellTotal = 0;

                var rowTotal = sheet.CreateRow(rowIndex);
                rowTotal.CreateCell(indexCellTotal).SetCellValue(reportTotalView.TimeTotal.DisplayNameTimeActualTotal);

                if (reportTotalView.DisplayNames.DisplayNameDate != null || reportTotalView.GroupByTypeId == (int)Constants.ReportsGroupBy.Date)
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
                rowTotal.CreateCell(++indexCellTotal).SetCellValue(actualTotalValue);

                if (reportTotalView.DisplayNames.DisplayNameTimeEstimated != null)
                {
                    var totalEst = reportTotalView.TimeTotal.TimeEstimatedTotal == 0 ? null : ConvertModelToView.UpdateTimeFormatForValue(reportTotalView.TimeTotal.TimeEstimatedTotal.ToString());
                    rowTotal.CreateCell(++indexCellTotal).SetCellValue(totalEst);
                }

                if (reportTotalView.DisplayNames.DisplayNameNotes != null)
                {
                    rowTotal.CreateCell(++indexCellTotal).SetCellValue(string.Empty);
                }

                #region EX

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

                #endregion

                workbook.Write(memoryStream);

                return memoryStream.ToArray();
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