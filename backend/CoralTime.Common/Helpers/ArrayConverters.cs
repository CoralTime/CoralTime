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
    }
}
