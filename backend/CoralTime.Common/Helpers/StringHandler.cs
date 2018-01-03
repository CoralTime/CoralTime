using System.Text;

namespace CoralTime.Common.Helpers
{
    public class StringHandler
    {

        public static string SeparateStringByUpperCase(string str)
        {
            char[] chars = str.ToCharArray();
            StringBuilder result = new StringBuilder();

            result.Append(chars[0]);

            for (int i = 1; i < chars.Length; i++)
            {
                if (char.IsUpper(chars[i]))
                {
                    result.Append(' ');
                    result.Append(char.ToLower(chars[i]));
                }
                else
                {
                    result.Append(chars[i]);
                }
            }

            return result.ToString();
        }

        public static string ToLowerCamelCase(string name)
        {
            return name.Substring(0, 1).ToLower() + name.Substring(1);
        }
    }
}
