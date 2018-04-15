using System;
using System.Linq;

namespace CoralTime.Common.Helpers
{
    public partial class CommonHelpers
    {
        public static string ConvertFromArrayOfIntsToString(int[] array)
        {
            return array != null ? string.Join(",", array) : null;
        }

        public static string ConvertFromArrayOfNullableIntsToString(int?[] array)
        {
            return array != null ? string.Join(",", array) : null;
        }

        public static int[] ConvertStringToArrayOfInts(string sourceString)
        {
            return !string.IsNullOrEmpty(sourceString)
                ? sourceString.Split(',').Select(int.Parse).ToArray()
                : null;
        }

        public static int?[] ConvertStringToArrayOfNullableInts(string sourceString)
        {
            return !string.IsNullOrEmpty(sourceString)
                ? sourceString.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => (int?)Convert.ToInt32(x)).ToArray()
                : null;
        }

    }
}
