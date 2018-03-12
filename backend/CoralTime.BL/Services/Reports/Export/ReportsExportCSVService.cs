using CoralTime.ViewModels.Reports;
using System.Collections.Generic;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        //private byte[] CreateFileCSV<T>(IReportsGrandGridView<T> data)
        //{
        //    if (!RunSetCommonValuesForExport)
        //    {
        //        throw new InvalidOperationException("You forgot run SetCommonValuesForExport() for set common values.");
        //    }

        //    var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //    var sb = new StringBuilder();

        //    var tempRows = new string[0];

        //    foreach (var project in data.ReportsGridView)
        //    {
        //        var tempRow = new string[props.Length - 1];
        //        int index = 0;

        //        foreach (var prop in ExcludeProps(typeof(T)))
        //        {
        //            var type = prop.PropertyType;
        //            if (type == typeof(IEnumerable<ReportsGridItemsView>))
        //            {
        //                if (type == typeof(IEnumerable<ReportsGridItemsView>))
        //                {
        //                    var value = (List<ReportsGridItemsView>) prop.GetValue(project, null);

        //                    tempRows = CreateItemsCSV(value);
        //                }
        //            }
        //            else
        //            {
        //                var value = GetNameDisplayForGrandAndTotalHeaders(prop.Name) + prop.GetValue(project, null);

        //                tempRow[index] = value;
        //                index++;
        //            }
        //        }

        //        sb.AppendLine(string.Join(",", tempRow));

        //        foreach (var row in tempRows)
        //        {
        //            sb.AppendLine(string.Join(",", row));
        //        }

        //        sb.AppendLine();
        //    }

        //    // 3. Send responce with file.
        //    return Encoding.UTF8.GetBytes(sb.ToString());
        //}

        //private string[] CreateItemsCSV(List<ReportsGridItemsView> items)
        //{
        //    var result = new string[items.Count + 1];
        //    var props = ExcludeProps(typeof(ReportsGridItemsView));

        //    var headers = RenameNestedEntityHeaders(props);

        //    result[0] = string.Join(",", headers);

        //    int index = 1;
        //    foreach (var item in items)
        //    {
        //        var fields = new List<string>();

        //        foreach (var prop in props)
        //        {
        //            if (IsGroupByThisProperty(prop.Name))
        //            {
        //                continue;
        //            }

        //            var value = prop.GetValue(item, null) ?? string.Empty;
        //            fields.Add(value.ToString());
        //        }

        //        result[index] = string.Join(",", fields);
        //        index++;
        //    }

        //    return result;
        //}

    }
}