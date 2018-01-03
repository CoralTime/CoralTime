using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Reports;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportExportService
    {
        private byte[] CreateFileExcel<T>(IReportsGrandGridView<T> data)
        {
            if (!RunSetCommonValuesForExport)
            {
                throw new InvalidOperationException("You forgot run SetCommonValuesForExport() for set common values.");
            }

            using (var spreadsheetStream = new MemoryStream())
            {
                spreadsheetStream.Seek(0, SeekOrigin.Begin);

                using (var spreadsheetDocument = SpreadsheetDocument.Create(spreadsheetStream, SpreadsheetDocumentType.Workbook, true))
                {
                    // Add a WorkbookPart to the document.
                    var workbookpart = spreadsheetDocument.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();

                    // Add a WorksheetPart to the WorkbookPart.
                    var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

                    AddSheets(spreadsheetDocument, worksheetPart);
                    AddStyle(workbookpart);

                    var ws = new Worksheet();

                    SetColumnWidth(ws);

                    //Add Sheet Data.
                    // Get the SharedStringTablePart. (only for set string type of data, other type working is good).
                    var shareStringPart = spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();
                    var sheetData = CreateSheetData(data, shareStringPart);

                    #region Merge Cells

                    //var mergeCells = new MergeCells();

                    //mergeCells.Append(new MergeCell { Reference = new StringValue("B3:D3") });
                    //ws.InsertAfter(mergeCells, sheetData);

                    #endregion

                    ws.Append(sheetData);

                    worksheetPart.Worksheet = ws;
                    workbookpart.Workbook.Save();
                }

                return spreadsheetStream.ToArray();
            }
        }

        #region Add Sheets.

        private void AddSheets(SpreadsheetDocument spreadsheetDocument, WorksheetPart worksheetPart)
        {
            // Add Sheets to the Workbook.
            var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            var sheet = new Sheet
            {
                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Group By " + GetDescriptionGroupById(GroupById)
            };
            sheets.Append(sheet);
        }

        private string GetDescriptionGroupById(int groupById)
        {
            switch (groupById)
            {
                case 0:
                {
                    return Constants.ReportsGroupBy.None.ToString();
                }

                case 1:
                {
                    return Constants.ReportsGroupBy.Project.ToString();
                }

                case 2:
                {
                    return Constants.ReportsGroupBy.User.ToString();
                }

                case 3:
                {
                    return Constants.ReportsGroupBy.Date.ToString();
                }

                case 4:
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

        private void AddStyle(WorkbookPart workbookpart)
        {
            var wbsp = workbookpart.AddNewPart<WorkbookStylesPart>();
            wbsp.Stylesheet = new Stylesheet();

            wbsp.Stylesheet = CreateStyleSheet();
            wbsp.Stylesheet.Save();
        }

        private Stylesheet CreateStyleSheet()
        {
            var stylesheet = new Stylesheet();

            #region (Default) Font 0 settings (bold, 10ph, "Verdana", "Black").

            var font0 = new Font();

            font0.Append(new FontSize { Val = 10D });
            font0.Append(new FontName { Val = "Verdana" });
            font0.Append(new Color { Rgb = HexBinaryValue.FromString("FF000000") });

            #endregion

            #region (Info) Font 1 settings (bold, 10ph, "Arial", "Green").

            var font1 = new Font();

            font1.Append(new Bold());
            font1.Append(new FontSize { Val = 10D });
            font1.Append(new FontName { Val = "Arial" });
            font1.Append(new Color { Rgb = HexBinaryValue.FromString("FF6AA84F") });

            #endregion

            #region (Common headers) Font 2 settings (bold, 10ph, "Arial", "Blue" ).

            var font2 = new Font();

            font2.Append(new Bold());
            font2.Append(new FontSize { Val = 10D });
            font2.Append(new FontName { Val = "Arial" });
            font2.Append(new Color { Rgb = HexBinaryValue.FromString("FF099cce") });

            #endregion

            #region (Nested headers) Font 3 settings (bold, 10ph, "Arial", "Black" ).

            var font3 = new Font();

            font3.Append(new Bold());
            font3.Append(new FontSize { Val = 10D });
            font3.Append(new FontName { Val = "Arial" });
            font3.Append(new Color { Rgb = HexBinaryValue.FromString("FF000000") });

            #endregion

            // Add Fonts.
            var fonts = new Fonts();
            fonts.Append(font0);
            fonts.Append(font1);
            fonts.Append(font2);
            fonts.Append(font3);

            // Add fills.
            var fill0 = new Fill();
            var fill1 = new Fill(); // (Info)
            var fill2 = new Fill(); // (Common headers)
            var fill3 = new Fill(); // (Nested headers) 
            var fill4 = new Fill(); // (Nested rows) 

            var fills = new Fills();
            fills.Append(fill0);
            fills.Append(fill1);
            fills.Append(fill2);
            fills.Append(fill3);
            fills.Append(fill4);

            #region (Nested headers) Fill 3 (grey)
            //#f1f1f1
            var solidlightBlue = new PatternFill
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor
                {
                    Rgb = HexBinaryValue.FromString("FFf1f1f1")
                }
            };
            //solidlightBlue.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FF099cce") };
            //solidlightBlue.BackgroundColor = new BackgroundColor { Indexed = 64 };
            fill3.PatternFill = solidlightBlue;

            #endregion

            #region (Nested rows) Fill 4 (white)

            var solidWhite = new PatternFill
            {
                //PatternType = PatternValues.Solid
                ForegroundColor = new ForegroundColor
                {
                    Rgb = HexBinaryValue.FromString("FFFFFFFF")
                }
            };
            //solidWhite.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FFFFFFFF") };
            //solidWhite.BackgroundColor = new BackgroundColor { Indexed = 64 };
            fill4.PatternFill = solidWhite;

            #endregion

            //#region (Nested headers) Fill 3 (blue)
            ////#f1f1f1
            //var solidlightBlue = new PatternFill
            //{
            //    PatternType = PatternValues.Solid,
            //    ForegroundColor = new ForegroundColor
            //    {
            //        Rgb = HexBinaryValue.FromString("FF099cce")
            //    }
            //};
            ////solidlightBlue.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FF099cce") };
            ////solidlightBlue.BackgroundColor = new BackgroundColor { Indexed = 64 };
            //fill3.PatternFill = solidlightBlue;

            //#endregion
            // Add Borders.
            var border0 = new Border();
            var borders = new Borders();
            borders.Append(border0);

            // CellFormats.
            var verticalAligmentTop = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Top);
            var verticalAligmentBottom = new EnumValue<VerticalAlignmentValues>(VerticalAlignmentValues.Bottom);
            //var cellformat0 = new CellFormat { FontId = 0, FillId = 3, BorderId = 0 };
            var cellformat0 = new CellFormat { FontId = 0, FillId = 0, Alignment = new Alignment { Vertical = verticalAligmentTop, WrapText = true } };
            var cellformat1 = new CellFormat { FontId = 1, Alignment = new Alignment { Vertical = verticalAligmentTop, WrapText = true } }; // Info
            var cellformat2 = new CellFormat { FontId = 2, Alignment = new Alignment { Vertical = verticalAligmentBottom, WrapText = true } };  // (Common headers)
            var cellformat3 = new CellFormat { FontId = 3, FillId = 3, Alignment = new Alignment { WrapText = true } };  // (Nested headers) 
            var cellformat4 = new CellFormat { FontId = 0, Alignment = new Alignment { Vertical = verticalAligmentTop, WrapText = true } };  // (Nested rows) 
            var cellformat5 = new CellFormat { FontId = 0, Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Right, Vertical = verticalAligmentTop, WrapText = true } };  // (Nested rows, columns: Estimated, Actual) 
            var cellformat6 = new CellFormat { FontId = 0, Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Left, Vertical = verticalAligmentTop, WrapText = true } };  // (Nested rows, columns: Estimated, Actual) 

            // Add CellFormats.
            var cellformats = new CellFormats();
            cellformats.Append(cellformat0);
            cellformats.Append(cellformat1);
            cellformats.Append(cellformat2);
            cellformats.Append(cellformat3);
            cellformats.Append(cellformat4);
            cellformats.Append(cellformat5);
            cellformats.Append(cellformat6);

            // Add FONTS, FILLS, BORDERS & CellFormats to stylesheet. (Preserve the ORDER)
            stylesheet.Append(fonts);
            stylesheet.Append(fills);
            stylesheet.Append(borders);
            stylesheet.Append(cellformats);

            return stylesheet;
        }

        #endregion

        #region  Set column width.

        private void SetColumnWidth(Worksheet ws)
        {
            var sizeOne = 16;
            var sizeTwo = 26;
            var sizeThree = 44;

            uint countFirstColumns = 0;
            uint countSecondColumns = 0;
            uint countThirdColumns = 0;

            // Check count columns that can made be hide for each size.
            if (ShowColumnIds != null)
            {
                foreach (var showColumnInfo in showColumnsInfo)
                {
                    var isPropInArray = ShowColumnIds.Contains(showColumnInfo.Id);
                    if (isPropInArray)
                    {
                        var showColumnDescriptionsById = showColumnsInfo.FirstOrDefault(x => x.Id == showColumnInfo.Id)?.ShowColumnDescriptions;
                        foreach (var tmpItem in showColumnDescriptionsById)
                        {
                            if (tmpItem.Name == InternalProperties.Date.ToString()
                                || tmpItem.Name == InternalProperties.TimeFrom.ToString()
                                || tmpItem.Name == InternalProperties.TimeTo.ToString())
                            {
                                countFirstColumns++;
                                continue;
                            }

                            if (tmpItem.Name == InternalProperties.ActualTime.ToString()
                                || tmpItem.Name == InternalProperties.EstimatedTime.ToString())
                            {
                                countSecondColumns++;
                                continue;
                            }

                            if (tmpItem.Name == InternalProperties.Description.ToString())
                            {
                                countThirdColumns++;
                                continue;
                            }
                        }
                    }
                }
            }

            // Get First Size Count Columns.
            uint firstSizeStartColumn = 1;
            uint firstSizeFinishColumn = (uint)_alwaysShowProperty.Count + countFirstColumns;
            if (GroupById == (int) Constants.ReportsGroupBy.None)
            {
                ++firstSizeFinishColumn;
            }
        
            // When choose grouping by date and check "Hide Column Date".
            if (GroupById == (int)Constants.ReportsGroupBy.Date)
            {
                var dontShowDateColumn = false;

                var hideDateColumnId = showColumnsInfo.FirstOrDefault(x => x.ShowColumnDescriptions.Contains(x.ShowColumnDescriptions.FirstOrDefault(i => i.Name == InternalProperties.Description.ToString())))?.Id;
                if (hideDateColumnId != null)
                {
                    var hasNotDateColumnIdInTargetedIds = !ShowColumnIds?.Any(z => z == hideDateColumnId);
                    if (hasNotDateColumnIdInTargetedIds != null)
                    {
                        dontShowDateColumn = (bool)hasNotDateColumnIdInTargetedIds;
                    }
                }

                if (dontShowDateColumn)
                {
                    ++firstSizeFinishColumn;
                }
            }

            // Get Second Size Count Columns. 
            uint secondSizeStartColumn = firstSizeFinishColumn + 1;
            uint secondSizeFinishColumn = firstSizeFinishColumn + countSecondColumns;

            // Get Third Size Count Columns. 
            uint thirdSizeStartColumn = secondSizeFinishColumn + 1;
            uint thirdSizeFinishColumn = secondSizeFinishColumn + countThirdColumns;

            var columns = new Columns();

            // By default you must be set column width!
            columns.Append(CreateColumnData(firstSizeStartColumn, firstSizeFinishColumn, sizeOne));

            if (secondSizeStartColumn <= secondSizeFinishColumn && countSecondColumns > 0)
            {
                columns.Append(CreateColumnData(secondSizeStartColumn, secondSizeFinishColumn, sizeTwo));
            }

            if (thirdSizeStartColumn <= thirdSizeFinishColumn && countThirdColumns > 0)
            {
                columns.Append(CreateColumnData(thirdSizeStartColumn, thirdSizeFinishColumn, sizeThree));
            }

            ws.Append(columns);
        }

        private Column CreateColumnData(UInt32 StartColumnIndex, UInt32 EndColumnIndex, double ColumnWidth)
        {
            var column = new Column
            {
                Min = StartColumnIndex,
                Max = EndColumnIndex,
                Width = ColumnWidth,
                CustomWidth = true,
                //BestFit = true
            };

            return column;
        }

        #endregion

        #region Add Sheet Data.

        private SheetData CreateSheetData<T>(IReportsGrandGridView<T> groupedList, SharedStringTablePart shareStringPart)
        {
            var sheetData = new SheetData();

            AddPeriodDateAndGrandTimeRows(sheetData, groupedList, shareStringPart);

            var entityHeadersRow = new Row();
            var entityRows = new Row[0];

            foreach (var reportsGridView in groupedList.ReportsGridView)
            {
                var totalHeaderRow = new Row();

                foreach (var prop in PropsEntityHeaders)
                {
                    var propType = prop.PropertyType;

                    if (!propType.GetTypeInfo().IsGenericType)
                    {
                        // Total headers.
                        totalHeaderRow = CreateTotalHeadersRow(GetValueFromProp(prop, reportsGridView), shareStringPart, prop, totalHeaderRow);
                    }
                    else if(propType.GetTypeInfo().IsGenericType)
                    {
                        if (propType == typeof(IEnumerable<ReportsGridItemsView>))
                        {
                            // Entity Headers.
                            entityHeadersRow = CreateEntityHeadersRow(shareStringPart);

                            // Entity Rows.
                            entityRows = CreateEntityRows(GetListValueFromProp(prop, reportsGridView), shareStringPart);
                        }
                    }
                }

                sheetData.Append(totalHeaderRow);
                sheetData.Append(entityHeadersRow);
                sheetData.Append(entityRows);
                sheetData.Append(new Row());
            }

            return sheetData;
        }

        #region InfoRow

        private void AddPeriodDateAndGrandTimeRows<T>(SheetData sheetData, IReportsGrandGridView<T> groupedList, SharedStringTablePart shareStringPart)
        {
            var infoRow = new Row();

            infoRow = AddCellPeriodDate(infoRow);
            infoRow = AddEmptyCellsAfterInfoRow(infoRow);

            if (GroupById != (int)Constants.ReportsGroupBy.None)
            {
                infoRow = AddCellsGrandTimes(infoRow, groupedList, shareStringPart);
            }

            sheetData.Append(infoRow);
            sheetData.Append(new Row());
        }

        private Row AddEmptyCellsAfterInfoRow(Row infoRow)
        {
            var startVal = _alwaysShowProperty.Count - CountPeriodCells;
            var showPropertiesByIds = showColumnsInfo.Where(x => ShowColumnIds.Contains(x.Id));

            var showPropertiesBeforeActualEstimatedTime = showPropertiesByIds.Where(x => x.ShowColumnDescriptions
                .Any(z => z.Name == InternalProperties.Date.ToString() || z.Name == InternalProperties.TimeFrom.ToString() || z.Name == InternalProperties.TimeTo.ToString()));

            var dontShowDate = showPropertiesByIds.Count(x => x.ShowColumnDescriptions.Any(z => z.Name == InternalProperties.Date.ToString())) == 0;

            if (GroupById == (int)Constants.ReportsGroupBy.Date && dontShowDate)
            {
                ++startVal;
            }

            var countColumns = showPropertiesBeforeActualEstimatedTime.SelectMany(x => x.ShowColumnDescriptions).Count();
            startVal += countColumns;

            for (var i = 0; i < startVal; i++)
            {
                infoRow.Append(new Cell());
            }

            return infoRow;
        }

        private Row AddCellPeriodDate(Row infoRow)
        {
            var periodDateCell = new Cell();

            periodDateCell.InlineString = new InlineString(new Text(GetValueForPeriodCell()));
            periodDateCell.DataType = CellValues.InlineString;

            CountPeriodCells++;

            infoRow.Append(periodDateCell);
            return infoRow;
        }

        private Row AddCellsGrandTimes<T>(Row infoRow, IReportsGrandGridView<T> item, SharedStringTablePart shareStringPart)
        {
            foreach (var propGrand in PropsGrandHeaders)
            {
                if (!propGrand.PropertyType.GetTypeInfo().IsGenericType)
                {
                    var value = GetValueFromProp(propGrand, item);
                    value = UpdateTimeFormatForValue(propGrand, value);
                    var headerNameWithValue = ConcatHeaderNameWithValue(propGrand, value);

                    var grandCell = new Cell
                    {
                        CellValue = new CellValue(InsertSharedStringItem(headerNameWithValue, shareStringPart).ToString()),
                        DataType = new EnumValue<CellValues>(CellValues.SharedString),
                        StyleIndex = 2
                    };

                    infoRow.Append(grandCell);
                }
            }

            return infoRow;
        }

        #endregion

        #region  External properties of Entity.

        private string GetValueFromProp<T>(PropertyInfo prop, T item)
        {
            var valueFromProp = (prop.GetValue(item, null) ?? string.Empty).ToString();
            return valueFromProp;
        }

        private Row CreateTotalHeadersRow(string value, SharedStringTablePart shareStringPart, PropertyInfo prop, Row commonEntityRow)
        {
            var cell = new Cell();

            if (prop.PropertyType == typeof(DateTime))
            {
                var dateFormat = new GetDateFormat().GetDateFormaDotNetById(DateFormatId);
                value = DateTime.Parse(value).ToString(dateFormat, CultureInfo.InvariantCulture);
            }

            value = UpdateTimeFormatForValue(prop, value);
            value = UpdateProjectNameToUpperCase(prop, value);
            value = ConcatHeaderNameWithValue(prop, value);
            if (prop.Name == InternalProperties.TimeEntryName.ToString() && GroupById == (int)Constants.ReportsGroupBy.None)
            {
                value = string.Empty;
            }

            cell = GetCellValue(shareStringPart, value, cell);

            SetCellStyleForHeaders(prop, cell);

            commonEntityRow.Append(cell);

            AddEmptyCellsBeforCellTotalActualTime(prop, commonEntityRow);

            return commonEntityRow;
        }

        private string UpdateTimeFormatForValue(PropertyInfo prop, string value)
        {
            if (prop.Name == ExternalProperties.GrandActualTime.ToString()
                || prop.Name == ExternalProperties.GrandEstimatedTime.ToString()
                || prop.Name == ExternalProperties.TotalActualTime.ToString() 
                || prop.Name == ExternalProperties.TotalEstimatedTime.ToString()
                || prop.Name == InternalProperties.ActualTime.ToString()
                || prop.Name == InternalProperties.EstimatedTime.ToString()
                || prop.Name == InternalProperties.TimeFrom.ToString()
                || prop.Name == InternalProperties.TimeTo.ToString())
            {
                var time = TimeSpan.FromSeconds(Int32.Parse(value));
                if (time.TotalHours <= 99.99)
                {
                    value = $"{(int)time.TotalHours:D2}:{time.Minutes:D2}";
                }
                else
                {
                    value = $"{(int)time.TotalHours}:{time.Minutes:D2}";
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
            var headerNameWithValue = GetNameDisplayForGrandAndTotalHeaders(prop.Name) + value;
            
            if (prop.Name == InternalProperties.ClientName.ToString() && value == Constants.WithoutClient.Name)
            {
                headerNameWithValue = value;
            }

            return headerNameWithValue;
        }

        private void SetCellStyleForHeaders(PropertyInfo prop, Cell cell)
        {
            if (prop.Name == ExternalProperties.ProjectName.ToString()
                || prop.Name == ExternalProperties.TotalActualTime.ToString()
                || prop.Name == ExternalProperties.TotalEstimatedTime.ToString()
                || prop.Name == ExternalProperties.ClientName.ToString()
                || prop.Name == ExternalProperties.MemberName.ToString()
                || prop.Name == ExternalProperties.Date.ToString())
            {
                cell.StyleIndex = 2;
            }
        }
        
        private void AddEmptyCellsBeforCellTotalActualTime(PropertyInfo prop, Row entityRow)
        {
            var startVal = _alwaysShowProperty.Count - CountGroupByCells;

            if (GroupById == (int)Constants.ReportsGroupBy.None)
            {
                ++startVal;
            }

            if (IsPropByDefaultGrouping(prop.Name))
            {
                var showPropertiesByIds = showColumnsInfo.Where(x => ShowColumnIds.Contains(x.Id));

                var showPropsBeforeActualEstimatedTime = showPropertiesByIds.Where(x => x.ShowColumnDescriptions
                    .Any(z => z.Name == InternalProperties.Date.ToString() || z.Name == InternalProperties.TimeFrom.ToString() || z.Name == InternalProperties.TimeTo.ToString()));

                var dontShowDate = showPropertiesByIds.Count(x => x.ShowColumnDescriptions.Any(z => z.Name == InternalProperties.Date.ToString())) == 0;

                if (GroupById == (int)Constants.ReportsGroupBy.Date && dontShowDate)
                {
                    ++startVal;
                }

                var countColumns = showPropsBeforeActualEstimatedTime.SelectMany(x => x.ShowColumnDescriptions).Count();
                
                startVal += countColumns;

                for (int i = 0; i < startVal; i++)
                {
                    entityRow.Append(new Cell());
                }
            }
        }

        #endregion

        #region Work with inner List<ReportGridItemView> of entities and its properties. And check if field is generic.

        private IEnumerable<ReportsGridItemsView> GetListValueFromProp<T>(PropertyInfo prop, T item)
        {
            var getListValueFromProp = (IEnumerable<ReportsGridItemsView>)prop.GetValue(item, null);
            return getListValueFromProp;
        }

        private Row CreateEntityHeadersRow(SharedStringTablePart shareStringPart)
        {
            var headersRow = new Row();
            var nestedEntityHeaders = RenameNestedEntityHeaders(PropsEntityRows);

            foreach (var header in nestedEntityHeaders)
            {
                var headCell = new Cell();
                var value = header;

                headCell = GetCellValue(shareStringPart, value, headCell);
                headCell.StyleIndex = 3;
                headersRow.Append(headCell);
            }

            return headersRow;
        }

        private Row[] CreateEntityRows(IEnumerable<ReportsGridItemsView> nestedEntities, SharedStringTablePart shareStringPart)
        {
            var nestedEntityRows = new Row[nestedEntities.Count()]; 

            #region Nested entity rows.

            var index = 0;
            foreach (var item in nestedEntities)
            {
                var row = new Row();

                foreach (var prop in PropsEntityRows)
                {
                    if (!IsPropByDefaultGrouping(prop.Name))
                    {
                        var cell = new Cell();
                        var value = (prop.GetValue(item, null) ?? string.Empty).ToString();

                        cell.StyleIndex = 0;

                        if (prop.PropertyType == typeof(DateTime))
                        {
                            var dateFormat = new GetDateFormat().GetDateFormaDotNetById(DateFormatId);
                            value = DateTime.Parse(value).ToString(dateFormat);
                        }

                        if (prop.Name == InternalProperties.ActualTime.ToString()
                            || prop.Name == InternalProperties.EstimatedTime.ToString()
                            || prop.Name == InternalProperties.TimeFrom.ToString()
                            || prop.Name == InternalProperties.TimeTo.ToString())
                        {
                            var time = TimeSpan.FromSeconds(Int32.Parse(value));
                            value = time.ToString(@"hh\:mm");
                        }

                        if (prop.Name == InternalProperties.TotalActualTime.ToString() ||    
                            prop.Name == InternalProperties.TotalEstimatedTime.ToString())
                        {
                            cell = GetCellValue(shareStringPart, value, cell);
                            cell.StyleIndex = 5;
                        }
                        else if (prop.Name == InternalProperties.TaskName.ToString() ||
                                 prop.Name == InternalProperties.Description.ToString())
                        {
                            cell = GetCellValue(shareStringPart, value, cell);
                            cell.StyleIndex = 6;
                        }
                        else
                        {
                            cell = GetCellValue(shareStringPart, value, cell);
                        }

                        row.Append(cell);
                    }
                }

                nestedEntityRows[index++] = row;
            }

            #endregion

            return nestedEntityRows;
        }

        #endregion

        #region Common methods.
        
        private Cell GetCellValue(SharedStringTablePart shareStringPart, string value, Cell cell)
        {
            int mockUpValue;
            if (Int32.TryParse(value, out mockUpValue))
            {
                cell.CellValue = new CellValue(value);
            }
            else
            {
                cell.CellValue = new CellValue(InsertSharedStringItem(value, shareStringPart).ToString());
                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
            }

            cell.StyleIndex = 4;
            return cell;
        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        #endregion

        #endregion
    }
}