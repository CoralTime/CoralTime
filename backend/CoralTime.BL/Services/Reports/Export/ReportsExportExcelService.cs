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
            if (!RunSetCommonValuesForExport)
            {
                throw new InvalidOperationException("You forgot run SetCommonValuesForExport() for set common values.");
            }

            using (var memoryStream = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();

                AddFilledSheet(groupingList, workbook);
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

        private void AddFilledSheet(DataSet dataSet, IWorkbook workbook)
        {
            var sheet1 = workbook.CreateSheet("Group By " + GetDescriptionGroupById(GroupById));

            // INFO 
            var rowIndex = 0;
            var rowInfo = sheet1.CreateRow(rowIndex);
            var cellInfo1 = rowInfo.CreateCell(0);
            cellInfo1.SetCellValue(dataSet.DataSetName);

            rowIndex += 2;

            for (int ii = 0; ii < dataSet.Tables.Count; ii++)
            {
                var dt = dataSet.Tables[ii];

                // GROUP BY
                var rowTableName = sheet1.CreateRow(rowIndex);
                var cellTableName = rowTableName.CreateCell(0);
                cellTableName.SetCellValue(dt.TableName);

                ++rowIndex;

                // HEADERS
                var rowHeaders = sheet1.CreateRow(rowIndex);
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    var cell = rowHeaders.CreateCell(j);
                    var columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                }

                ++rowIndex;

                // VALUES
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var rowHeaderValues = sheet1.CreateRow(rowIndex);
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        var cell = rowHeaderValues.CreateCell(j);
                        var columnName = dt.Columns[j].ToString();
                        cell.SetCellValue(dt.Rows[i][columnName].ToString());
                    }

                    ++rowIndex;
                }

                // TOTAL FOR 
                var rowTotalForTableName = sheet1.CreateRow(rowIndex);
                var cellTotalForTableName = rowTotalForTableName.CreateCell(0);
                cellTotalForTableName.SetCellValue("TOTAL FOR: " + dt.TableName);

                ++rowIndex;

                ++rowIndex;
            }
        }

        #region Add Sheets.

        //private void AddSheets(SpreadsheetDocument spreadsheetDocument, WorksheetPart worksheetPart)
        //{
        //    // Add Sheets to the Workbook.
        //    var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

        //    // Append a new worksheet and associate it with the workbook.
        //    var sheet = new Sheet
        //    {
        //        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
        //        SheetId = 1,
        //        Name = "Group By " + GetDescriptionGroupById(GroupById)
        //    };

        //    sheets.Append(sheet);
        //}

        private string GetDescriptionGroupById(int groupById)
        {
            switch (groupById)
            {
                case (int) Constants.ReportsGroupBy.None:
                {
                    return Constants.ReportsGroupBy.None.ToString();
                }

                case (int) Constants.ReportsGroupBy.Project:
                {
                    return Constants.ReportsGroupBy.Project.ToString();
                }

                case (int) Constants.ReportsGroupBy.User:
                {
                    return Constants.ReportsGroupBy.User.ToString();
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

        #endregion

        #region  Add styles.

        //private void AddStyle(WorkbookPart workbookpart)
        //{
        //    var wbsp = workbookpart.AddNewPart<WorkbookStylesPart>();
        //    wbsp.Stylesheet = new Stylesheet();

        //    wbsp.Stylesheet = CreateStyleSheet();
        //    wbsp.Stylesheet.Save();
        //}

        private enum FormatsForCells
        {
            RowHeaderNameTotalTotalForGroupBy = 0,
            RowHeaderValueTotalTotalFor = 1,
            RowHeaderNamesEntity = 2,
            RowHeaderValuesEntity = 3
        }

        private enum FontsFormatForCells
        {
            RowHeaderNameTotalTotalForGroupBy = 0,
            RowHeaderValueTotalTotalFor = 1,
            RowHeaderNamesEntity = 2,
            RowHeaderValuesEntity = 3
        }

        //private Stylesheet CreateStyleSheet()
        //{
        //    var stylesheet = new Stylesheet();

        //    var fontNameRobotoRegular = "Roboto Regular";

        //    var fontSize10D = 10D;

        //    var fontColorBlue = "FF099cce";
        //    var fontColorBlack = "FF000000";
        //    var fontColorLightGrey = "FF404040";

        //    var fontBold = new Bold();

        //    #region Font 0 settings (10px, "RobotoRegular", "LightGrey") (RowsHeadersValuesEntity) 

        //    var font0 = new Font();

        //    font0.Append(new FontSize { Val = fontSize10D });
        //    font0.Append(new FontName { Val = fontNameRobotoRegular });
        //    font0.Append(new Color { Rgb = HexBinaryValue.FromString(fontColorLightGrey) });

        //    #endregion

        //    #region Font 1 settings (10px, "RobotoRegular", "Blue") (RowHeaderNameTotalFor  + RowHeaderNameTotal + RowHeaderNameGroupByDefault ) 

        //    var font1 = new Font();

        //    font1.Append(new FontSize { Val = fontSize10D });
        //    font1.Append(new FontName { Val = fontNameRobotoRegular });
        //    font1.Append(new Color { Rgb = HexBinaryValue.FromString(fontColorBlue) });

        //    #endregion

        //    #region Font 2 settings (10px, "RobotoRegular", "Black") (Row period date) 

        //    var font2 = new Font();

        //    font2.Append(new Bold());
        //    font2.Append(new FontSize { Val = fontSize10D });
        //    font2.Append(new FontName { Val = fontNameRobotoRegular });
        //    font2.Append(new Color { Rgb = HexBinaryValue.FromString(fontColorBlack) });

        //    #endregion

        //    #region Font 3 settings (10px, "RobotoRegular", "Black", bold) (Row entity headers names + Row total HeaderValue + Row totalFor HeaderValue) 

        //    var font3 = new Font();

        //    //font3.Append(fontBold);
        //    font3.Append(new FontSize { Val = fontSize10D });
        //    font3.Append(new FontName { Val = fontNameRobotoRegular });
        //    font3.Append(new Color { Rgb = HexBinaryValue.FromString(fontColorBlack) });

        //    #endregion

        //    #region Add fonts, fills, borders.

        //    // Add Fonts.
        //    var fonts = new Fonts();
        //    fonts.Append(font0);
        //    fonts.Append(font1);
        //    fonts.Append(font2);
        //    fonts.Append(font3);

        //    // Add fills.
        //    var fill0 = new Fill();
        //    var fill1 = new Fill(); // 
        //    var fill2 = new Fill(); // 
        //    var fill3 = new Fill(); //  
        //    var fill4 = new Fill(); // 

        //    var fills = new Fills();
        //    fills.Append(fill0);
        //    fills.Append(fill1);
        //    fills.Append(fill2);
        //    fills.Append(fill3);
        //    fills.Append(fill4);

        //    #region (Nested headers) Fill 3 (grey)

        //    var backgroundColorLightGray = "FFf1f1f1"; //#f1f1f1
        //    var solidlightBlue = new PatternFill
        //    {
        //        PatternType = PatternValues.Solid,
        //        ForegroundColor = new ForegroundColor
        //        {
        //            Rgb = HexBinaryValue.FromString(backgroundColorLightGray)
        //        }
        //    };
        //    //solidlightBlue.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FF099cce") };
        //    //solidlightBlue.BackgroundColor = new BackgroundColor { Indexed = 64 };
        //    fill3.PatternFill = solidlightBlue;

        //    #endregion

        //    #region (Nested rows) Fill 4 (white)

        //    var solidWhite = new PatternFill
        //    {
        //        //PatternType = PatternValues.Solid
        //        ForegroundColor = new ForegroundColor
        //        {
        //            Rgb = HexBinaryValue.FromString("FFFFFFFF")
        //        }
        //    };
        //    //solidWhite.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FFFFFFFF") };
        //    //solidWhite.BackgroundColor = new BackgroundColor { Indexed = 64 };
        //    fill4.PatternFill = solidWhite;

        //    #endregion

        //    //#region (Nested headers) Fill 3 (blue)
        //    ////#f1f1f1
        //    //var solidlightBlue = new PatternFill
        //    //{
        //    //    PatternType = PatternValues.Solid,
        //    //    ForegroundColor = new ForegroundColor
        //    //    {
        //    //        Rgb = HexBinaryValue.FromString("FF099cce")
        //    //    }
        //    //};
        //    ////solidlightBlue.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FF099cce") };
        //    ////solidlightBlue.BackgroundColor = new BackgroundColor { Indexed = 64 };
        //    //fill3.PatternFill = solidlightBlue;

        //    //#endregion
        //    // Add Borders.
        //    var border0 = new Border();
        //    var borders = new Borders();
        //    borders.Append(border0);

        //    #endregion

        //    var verticalAligmentTop = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Top);
        //    var verticalAligmentCenter = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Center);
        //    var verticalAligmentBottom = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Bottom);

        //    // CellFormats.
        //    var cellformat0 = new CellFormat { FontId = 0, Alignment = new Alignment { WrapText = true, Vertical = verticalAligmentCenter }, /*FillId = 0,*/ }; // RowHeaderNameGroupByDefault  + RowHeaderNameTotal + RowHeaderNameTotalFor
        //    var cellformat1 = new CellFormat { FontId = 1, Alignment = new Alignment { WrapText = true, Vertical = verticalAligmentCenter } }; // RowHeaderValueTotal + RowHeaderValueTotalFor 
        //    var cellformat2 = new CellFormat { FontId = 2, Alignment = new Alignment { WrapText = true, Vertical = verticalAligmentCenter }, FillId = 3, };  // RowHeaderNamesEntity
        //    var cellformat3 = new CellFormat { FontId = 2, Alignment = new Alignment { WrapText = true, Vertical = verticalAligmentCenter } }; // RowsHeadersValuesEntity
        //    var cellformat4 = new CellFormat { FontId = 0, Alignment = new Alignment { WrapText = true, Vertical = verticalAligmentCenter } };
        //    var cellformat5 = new CellFormat { FontId = 0, Alignment = new Alignment { WrapText = true, Vertical = verticalAligmentCenter, Horizontal = HorizontalAlignmentValues.Left, } }; // (Nested rows, columns: Estimated, Actual) 
        //    var cellformat6 = new CellFormat { FontId = 3, Alignment = new Alignment { WrapText = true, Vertical = verticalAligmentCenter, Horizontal = HorizontalAlignmentValues.Left, } }; // RowTotal HeaderValue + TotalFor HeaderValue

        //    // Add CellFormats.
        //    var cellformats = new CellFormats();
        //    cellformats.Append(cellformat0);
        //    cellformats.Append(cellformat1);
        //    cellformats.Append(cellformat2);
        //    cellformats.Append(cellformat3);
        //    cellformats.Append(cellformat4);
        //    cellformats.Append(cellformat5);
        //    cellformats.Append(cellformat6);

        //    // Add FONTS, FILLS, BORDERS & CellFormats to stylesheet. (Preserve the ORDER)
        //    stylesheet.Append(fonts);
        //    stylesheet.Append(fills);
        //    stylesheet.Append(borders);
        //    stylesheet.Append(cellformats);

        //    return stylesheet;
        //}

        #endregion

        #region  Set column width.

        //private void SetColumnWidth(Worksheet ws)
        //{
        //    var sizeOne = 30;
        //    var sizeTwo = 12;
        //    var sizeThree = 44; 

        //    uint countFirstColumns = 0;
        //    uint countSecondColumns = 1;
        //    uint countThirdColumns = 0;

        //    // Check count columns that can made be hide for each size.
        //    if (ShowColumnIds != null)
        //    {
        //        foreach (var showColumnInfo in showColumnsInfo)
        //        {
        //            var isPropInArray = ShowColumnIds.Contains(showColumnInfo.Id);
        //            if (isPropInArray)
        //            {
        //                var showColumnDescriptionsById = showColumnsInfo.FirstOrDefault(x => x.Id == showColumnInfo.Id)?.ShowColumnDescriptions;
        //                foreach (var tmpItem in showColumnDescriptionsById)
        //                {
        //                    //if (tmpItem.Name == InternalProperties.Date.ToString())
        //                    //{
        //                    //    countFirstColumns++;
        //                    //    continue;
        //                    //}

        //                    if (tmpItem.Name == InternalProperties.ActualTime.ToString() 
        //                        || tmpItem.Name == InternalProperties.EstimatedTime.ToString()
        //                        || tmpItem.Name == InternalProperties.TimeFrom.ToString()
        //                        || tmpItem.Name == InternalProperties.TimeTo.ToString()
        //                        || tmpItem.Name == InternalProperties.Date.ToString()
        //                        )
        //                    {
        //                        countSecondColumns++;
        //                        continue;
        //                    }

        //                    if (tmpItem.Name == InternalProperties.Description.ToString())
        //                    {
        //                        countThirdColumns++;
        //                        continue;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    // Get First Size Count Columns.
        //    uint firstSizeStartColumn = 1;
        //    uint firstSizeFinishColumn = (uint)_alwaysShowProperty.Count + countFirstColumns;
        //    if (GroupById == (int) Constants.ReportsGroupBy.None)
        //    {
        //        ++firstSizeFinishColumn;
        //    }

        //    // When choose grouping by date and check "Hide Column Date".
        //    if (GroupById == (int)Constants.ReportsGroupBy.Date)
        //    {
        //        var dontShowDateColumn = false;

        //        var hideDateColumnId = showColumnsInfo.FirstOrDefault(x => x.ShowColumnDescriptions.Contains(x.ShowColumnDescriptions.FirstOrDefault(i => i.Name == InternalProperties.Date.ToString())))?.Id;
        //        if (hideDateColumnId != null)
        //        {
        //            var hasNotDateColumnIdInTargetedIds = !ShowColumnIds?.Any(z => z == hideDateColumnId);
        //            if (hasNotDateColumnIdInTargetedIds != null)
        //            {
        //                dontShowDateColumn = (bool)hasNotDateColumnIdInTargetedIds;
        //            }
        //        }

        //        ++firstSizeFinishColumn;

        //        if (!dontShowDateColumn)
        //        {
        //            --countSecondColumns;
        //        }
        //    }

        //    // Get Second Size Count Columns. 
        //    uint secondSizeStartColumn = firstSizeFinishColumn + 1;
        //    uint secondSizeFinishColumn = firstSizeFinishColumn + countSecondColumns;

        //    // Get Third Size Count Columns. 
        //    uint thirdSizeStartColumn = secondSizeFinishColumn + 1;
        //    uint thirdSizeFinishColumn = secondSizeFinishColumn + countThirdColumns;

        //    var columns = new Columns();

        //    // By default you must be set column width!
        //    columns.Append(CreateColumnData(firstSizeStartColumn, firstSizeFinishColumn, sizeOne));

        //    if (secondSizeStartColumn <= secondSizeFinishColumn && countSecondColumns > 0)
        //    {
        //        columns.Append(CreateColumnData(secondSizeStartColumn, secondSizeFinishColumn, sizeTwo));
        //    }

        //    if (thirdSizeStartColumn <= thirdSizeFinishColumn && countThirdColumns > 0)
        //    {
        //        columns.Append(CreateColumnData(thirdSizeStartColumn, thirdSizeFinishColumn, sizeThree));
        //    }

        //    ws.Append(columns);
        //}

        private Column CreateColumnData(UInt32 StartColumnIndex, UInt32 EndColumnIndex, double ColumnWidth)
        {
            var column = new Column
            {
                //Min = StartColumnIndex,
                //Max = EndColumnIndex,
                //Width = ColumnWidth,
                //CustomWidth = true,
                //BestFit = true
            };

            return column;
        }

        #endregion

        #region Add Sheet Data.

        //private SheetData CreateSheetData<T>(IReportsGrandGridView<T> groupingList, SharedStringTablePart shareStringPart)
        //{
        //    var sheetData = new SheetData();

        //    CreateRowPeriodDate(sheetData);

        //    foreach (var reportsGridView in groupingList.ReportsGridView)
        //    {
        //        var rowGroupByWithValue = new Row();

        //        foreach (var propGroupByAndTotalTime in PropsGroupByAndTotalTimes)
        //        {
        //            var propType = propGroupByAndTotalTime.PropertyType;

        //            if (!propType.GetTypeInfo().IsGenericType && IsPropByDefaultGrouping(propGroupByAndTotalTime.Name))
        //            {
        //                // Row GroupBy "PROJECT: 3 MIDDLE PROJECT".
        //                var valueSingleFromProp = GetValueSingleFromProp(propGroupByAndTotalTime, reportsGridView);
        //                CreateRowGroupByWithValue(sheetData, valueSingleFromProp, shareStringPart, propGroupByAndTotalTime, rowGroupByWithValue);
        //            }
        //            else if(propType.GetTypeInfo().IsGenericType)
        //            {
        //                if (propType == typeof(IEnumerable<ReportsGridItemsView>))
        //                {
        //                    // Row Entity Header Names.
        //                    CreateRowEntityHeaderNames(shareStringPart, sheetData);

        //                    // Rows Entity Header Values.
        //                    var valueListFromProp = GetValueListFromProp(propGroupByAndTotalTime, reportsGridView);
        //                    CreateRowsEntityValuesAndTotalFor(sheetData, valueListFromProp, shareStringPart, propGroupByAndTotalTime, reportsGridView);
        //                }
        //            }
        //        }
                
        //        sheetData.Append(new Row());
        //    }

        //    // TOTAL
        //    CreateRowTotal(groupingList, shareStringPart, sheetData);

        //    return sheetData;
        //}

        #region Create Row PeriodDate.

        //private void CreateRowPeriodDate(SheetData sheetData)
        //{
        //    var rowPeriodDate = new Row();

        //    var cellPeriodDate = new Cell();

        //    cellPeriodDate.InlineString = new InlineString(new Text(GetValueForCellPeriodDate()));
        //    cellPeriodDate.DataType = CellValues.InlineString;

        //    //CountPeriodCells++;

        //    rowPeriodDate.Append(cellPeriodDate);

        //    sheetData.Append(rowPeriodDate);
        //    sheetData.Append(new Row());
        //}

        #endregion

        //private Row CreateEmptyCellsAfterTotalForCell(Row totalRow)
        //{
        //    var startVal = _alwaysShowProperty.Count - CountPeriodCells;
        //    var showPropertiesByIds = showColumnsInfo.Where(x => ShowColumnIds.Contains(x.Id));

        //    var showPropertiesBeforeActualEstimatedTime = showPropertiesByIds.Where(x => x.ShowColumnDescriptions
        //        .Any(z => z.Name == InternalProperties.Date.ToString() || z.Name == InternalProperties.TimeFrom.ToString() || z.Name == InternalProperties.TimeTo.ToString()));

        //    var dontShowDate = showPropertiesByIds.Count(x => x.ShowColumnDescriptions.Any(z => z.Name == InternalProperties.Date.ToString())) == 0;

        //    if (GroupById == (int)Constants.ReportsGroupBy.Date && dontShowDate)
        //    {
        //        ++startVal;
        //    }

        //    var countColumns = showPropertiesBeforeActualEstimatedTime.SelectMany(x => x.ShowColumnDescriptions).Count();
        //    startVal += countColumns;

        //    for (var i = 0; i < startVal; i++)
        //    {
        //        totalRow.Append(new Cell());
        //    }

        //    return totalRow;
        //}

        //private Row CreateRowTotalHeaderName(Row totalRow)
        //{
        //    var totalCell = new Cell();

        //    totalCell.InlineString = new InlineString(new Text(GetValueForTotaldCell()));
        //    totalCell.DataType = CellValues.InlineString;
        //    totalCell.StyleIndex = 1;

        //    CountPeriodCells++;

        //    totalRow.Append(totalCell);
        //    return totalRow;
        //}

        //private Row CreateRowTotalFor(Row rowTotalFor, string valueSingleFromProp)
        //{
        //    var totalInfoCell = new Cell();
        //    totalInfoCell.InlineString = new InlineString(new Text("TOTAL FOR: " + valueSingleFromProp));
        //    totalInfoCell.DataType = CellValues.InlineString;
        //    totalInfoCell.StyleIndex = 1;

        //    rowTotalFor.Append(totalInfoCell);

        //    return rowTotalFor;
        //}

        //private Row CreateRowTotalHeaderValue<T>(Row rowTotal, IReportsGrandGridView<T> item, SharedStringTablePart shareStringPart)
        //{
        //    foreach (var propTotal in PropsEntitiesTotalHeaders)
        //    {
        //        if (!propTotal.PropertyType.GetTypeInfo().IsGenericType)
        //        {
        //            var value = GetValueSingleFromProp(propTotal, item);
        //            value = UpdateTimeFormatForValue(propTotal, value);

        //            var grandCell = new Cell
        //            {
        //                CellValue = new CellValue(InsertSharedStringItem(value, shareStringPart).ToString()),
        //                DataType = new EnumValue<CellValues>(CellValues.SharedString),
        //                StyleIndex = 3
        //            };

        //            rowTotal.Append(grandCell);
        //        }
        //    }

        //    return rowTotal;
        //}

        //private void CreateRowTotal<T>(IReportsGrandGridView<T> groupingList, SharedStringTablePart shareStringPart, SheetData sheetData)
        //{
        //    if (GroupById != (int)Constants.ReportsGroupBy.None)
        //    {
        //        var rowTotal = new Row();

        //        rowTotal = CreateRowTotalHeaderName(rowTotal);
        //        rowTotal = CreateEmptyCellsAfterTotalForCell(rowTotal);
        //        rowTotal = CreateRowTotalHeaderValue(rowTotal, groupingList, shareStringPart);
        //        sheetData.Append(rowTotal);
        //    }
        //}

        #region  External properties of Entity.

        private string GetValueSingleFromProp<T>(PropertyInfo prop, T item)
        {
            var valueFromProp = (prop.GetValue(item, null) ?? string.Empty).ToString();
            return valueFromProp;
        }

        private string CreateTableNameFromGroupByHeader(string valueSingleFromProp, PropertyInfo prop)
        {
            valueSingleFromProp = UpdateDateFormatForValue(prop, valueSingleFromProp);
            valueSingleFromProp = UpdateTimeFormatForValue(prop, valueSingleFromProp);
            valueSingleFromProp = ConcatHeaderNameWithValue(prop, valueSingleFromProp).ToUpper();

            return valueSingleFromProp;
        }

        private string UpdateDateFormatForValue(PropertyInfo prop, string valueSingleFromProp)
        {
            if (prop.PropertyType == typeof(DateTime))
            {
                var dateFormat = new GetDateFormat().GetDateFormaDotNetById(DateFormatId);
                valueSingleFromProp = DateTime.Parse(valueSingleFromProp).ToString(dateFormat, CultureInfo.InvariantCulture);
            }

            return valueSingleFromProp;
        }

        private string UpdateTimeFormatForValue(PropertyInfo prop, string value)
        {
            if (prop.Name == ExternalProperties.TotalActualTime.ToString()
                || prop.Name == ExternalProperties.TotalEstimatedTime.ToString()
                || prop.Name == ExternalProperties.TotalForActualTime.ToString() 
                || prop.Name == ExternalProperties.TotalForEstimatedTime.ToString()
                || prop.Name == InternalProperties.ActualTime.ToString()
                || prop.Name == InternalProperties.EstimatedTime.ToString()
                || prop.Name == InternalProperties.TimeFrom.ToString()
                || prop.Name == InternalProperties.TimeTo.ToString())
            {
                var time = TimeSpan.FromSeconds(Int32.Parse(value));
                if (time.TotalHours == 0)
                {
                    value = string.Empty;
                }
                else
                {
                    if (time.TotalHours <= 99.99)
                    {
                        value = $"{(int) time.TotalHours:D2}:{time.Minutes:D2}";
                    }
                    else
                    {
                        value = $"{(int) time.TotalHours}:{time.Minutes:D2}";
                    }
                }
            }

            return value;
        }

        private string UpdateProjectNameToUpperCase(PropertyInfo prop, string value)
        {
            if (prop.Name == InternalProperties.ProjectName.ToString())
            {
                return value.ToUpper();
            }

            return value;
        }

        private string UpdateDateFormat(PropertyInfo prop, string value)
        {
            if (prop.PropertyType == typeof(DateTime))
            {
                var dateFormat = new GetDateFormat().GetDateFormaDotNetById(DateFormatId);
                value = DateTime.Parse(value).ToString(dateFormat);
            }

            return value;
        }

        private string ResetValueForGroupByNone(PropertyInfo prop, string value)
        {
            if (prop.Name == InternalProperties.TimeEntryName.ToString() && GroupById == (int)Constants.ReportsGroupBy.None)
            {
                value = string.Empty;
            }

            return value;
        }

        private string ConcatHeaderNameWithValue(PropertyInfo prop, string value)
        {
            var headerNameWithValue = string.Empty;

            // Remove text "Client:" for client with name = WithoutClient.
            if (prop.Name == InternalProperties.ClientName.ToString() && value == Constants.WithoutClient.Name)
            {
                headerNameWithValue = value;
            }
            else
            {
                headerNameWithValue = GetNameDisplayForGrandAndTotalHeaders(prop.Name) + value;
            }

            return headerNameWithValue;
        }

        //private void SetCellStyleForHeaders(PropertyInfo prop, Cell cell)
        //{
        //    if (prop.Name == ExternalProperties.ProjectName.ToString()
        //        || prop.Name == ExternalProperties.TotalActualTime.ToString()
        //        || prop.Name == ExternalProperties.TotalEstimatedTime.ToString()
        //        || prop.Name == ExternalProperties.ClientName.ToString()
        //        || prop.Name == ExternalProperties.MemberName.ToString()
        //        || prop.Name == ExternalProperties.Date.ToString())
        //    {
        //        cell.StyleIndex = 1;
        //    }
        //}
        
        //private void AddEmptyCellsBeforeTotalForCell(Row entityRow)
        //{
        //    var startVal = _alwaysShowProperty.Count - CountGroupByCells;

        //    if (GroupById == (int)Constants.ReportsGroupBy.None)
        //    {
        //        ++startVal;
        //    }

        //    var showPropertiesByIds = showColumnsInfo.Where(x => ShowColumnIds.Contains(x.Id));

        //    var showPropsBeforeActualEstimatedTime = showPropertiesByIds.Where(x => x.ShowColumnDescriptions
        //        .Any(z => z.Name == InternalProperties.Date.ToString() || z.Name == InternalProperties.TimeFrom.ToString() || z.Name == InternalProperties.TimeTo.ToString()));

        //    var dontShowDate = showPropertiesByIds.Count(x => x.ShowColumnDescriptions.Any(z => z.Name == InternalProperties.Date.ToString())) == 0;

        //    if (GroupById == (int)Constants.ReportsGroupBy.Date && dontShowDate)
        //    {
        //        ++startVal;
        //    }

        //    var countColumns = showPropsBeforeActualEstimatedTime.SelectMany(x => x.ShowColumnDescriptions).Count();
                
        //    startVal += countColumns;

        //    for (int i = 0; i < startVal; i++)
        //    {
        //        entityRow.Append(new Cell());
        //    }
        //}

        #endregion

        #region Work with inner List<ReportGridItemView> of entities and its properties. And check if field is generic.

        private IEnumerable<ReportsGridItemsView> GetValueListFromProp<T>(PropertyInfo prop, T item)
        {
            var getListValueFromProp = (IEnumerable<ReportsGridItemsView>)prop.GetValue(item, null);
            return getListValueFromProp;
        }

        //private void CreateRowEntityHeaderNames(SharedStringTablePart shareStringPart, SheetData sheetData)
        //{
        //    var rowEntityHeaderNames = new Row();
        //    var nestedEntityHeaders = RenameNestedEntityHeaders(PropsEntityHeadersAndRows);

        //    foreach (var header in nestedEntityHeaders)
        //    {
        //       CreateRowsEntityValuesAndTotalFor var headCell = new Cell();
        //        var value = header;

        //        headCell = GetCellValue(shareStringPart, value, headCell);
        //        headCell.StyleIndex = 2;
        //        rowEntityHeaderNames.Append(headCell);
        //    }

        //    sheetData.Append(rowEntityHeaderNames);
        //}

        //private void CreateRowsEntityValuesAndTotalFor<T>(SheetData sheetData, IEnumerable<ReportsGridItemsView> valueListFromProp,SharedStringTablePart shareStringPart, PropertyInfo propTotalFor, T reportsGridView)
        //{
        //    var rowsEntityValues = CreateRowsEntityValues(valueListFromProp, shareStringPart);
        //    var rowsEntityValuesAndTotalFor = new Row[rowsEntityValues.Length * 2];

        //    var index = 0;
        //    foreach (var rowEntityValues in rowsEntityValues)
        //    {
        //        // Add EntityRows
        //        rowsEntityValuesAndTotalFor[index] = rowEntityValues;

        //        // Add TotalFor Row
        //        rowsEntityValuesAndTotalFor[++index] = CreateTotalForRow(shareStringPart, reportsGridView);
        //    }

        //    sheetData.Append(rowsEntityValuesAndTotalFor);
        //}

        //private Row CreateTotalForRow<T>(SharedStringTablePart shareStringPart, T reportsGridView)
        //{
        //    var totalForRow = new Row();

        //    // Row TOTAL FOR Header Name.
        //    var propByDefaultGrouping = PropsGroupByAndTotalTimes.FirstOrDefault(x => !x.PropertyType.GetTypeInfo().IsGenericType && IsPropByDefaultGrouping(x.Name));
        //    var valueSingleFromProp = GetValueSingleFromProp(propByDefaultGrouping, reportsGridView);

        //    valueSingleFromProp = UpdateDateFormatForValue(valueSingleFromProp, propByDefaultGrouping).ToUpper();

        //    totalForRow = CreateRowTotalFor(totalForRow, valueSingleFromProp);
        //    //totalForRow = CreateEmptyCellsAfterTotalTime(totalForRow);
        //    AddEmptyCellsBeforeTotalForCell(totalForRow);

        //    // Row TOTAL FOR Header Values:
        //    var listOfPropsTotalActTime = PropsGroupByAndTotalTimes.Where(x => !x.PropertyType.GetTypeInfo().IsGenericType && IsPropTotalOrActualTime(x.Name));

        //    foreach (var propTimeFor in listOfPropsTotalActTime)
        //    {
        //        var valueSingleFromTotalActTime = GetValueSingleFromProp(propTimeFor, reportsGridView);
        //        valueSingleFromTotalActTime = UpdateTimeFormatForValue(propTimeFor, valueSingleFromTotalActTime);

        //        var cellTotalFor = new Cell();

        //        cellTotalFor = GetCellValue(shareStringPart, valueSingleFromTotalActTime, cellTotalFor);
        //        //SetCellStyleForHeaders(propTimeFor, cellTotalFor);
        //        cellTotalFor.StyleIndex = 3;

        //        totalForRow.Append(cellTotalFor);
        //    }

        //    return totalForRow;
        //}

        private void CreateRowsEntityValues(DataTable dataTable, IEnumerable<ReportsGridItemsView> valueListFromProp)
        {
            foreach (var item in valueListFromProp)
            {
                var dataRow = dataTable.NewRow();

                var valuesForRow = new List<string>();

                foreach (var prop in PropsEntityHeadersAndRows)
                {
                    if (!IsGroupByThisProperty(prop.Name))
                    {
                        var value = GetValueSingleFromProp(prop, item);

                        value = UpdateDateFormatForValue(prop, value);
                        value = UpdateTimeFormatForValue(prop, value);

                        valuesForRow.Add(value);
                    }
                }

                dataRow.ItemArray = valuesForRow.ToArray();
                dataTable.Rows.Add(dataRow);
            }
        }

        #endregion

        #region Common methods.

        //private Cell GetCellValue(SharedStringTablePart shareStringPart, string value, Cell cell)
        //{
        //    int mockUpValue;
        //    if (Int32.TryParse(value, out mockUpValue))
        //    {
        //        cell.CellValue = new CellValue(value);
        //    }
        //    else
        //    {
        //        cell.CellValue = new CellValue(InsertSharedStringItem(value, shareStringPart).ToString());
        //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //    }

        //    cell.StyleIndex = 4;
        //    return cell;
        //}

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        //private int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        //{
        //    // If the part does not contain a SharedStringTable, create one.
        //    if (shareStringPart.SharedStringTable == null)
        //    {
        //        shareStringPart.SharedStringTable = new SharedStringTable();
        //    }

        //    int i = 0;

        //    // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
        //    foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
        //    {
        //        if (item.InnerText == text)
        //        {
        //            return i;
        //        }

        //        i++;
        //    }

        //    // The text does not exist in the part. Create the SharedStringItem and return its index.
        //    shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
        //    shareStringPart.SharedStringTable.Save();

        //    return i;
        //}

        #endregion

        #endregion
    }
}