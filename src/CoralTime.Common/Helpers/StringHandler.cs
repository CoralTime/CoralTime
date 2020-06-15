using System.Text;
using System.Text.RegularExpressions;

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

        public static string RemoveMarkdown(string str)
        {
            // remove images
            var res = Regex.Replace(str, @"!\[[^\[\]]*?\]\(.*?\)", string.Empty);
            //remove urls
            res = Regex.Replace(res, @"\[[^\[\]]*?\]\(.*?\)|^\[*?\]\(.*?\)", string.Empty);
            // remove bold italic
            res = res.Replace("***", string.Empty);
            // remove bold
            res = res.Replace("**", string.Empty);
            // remove italic
            res = Regex.Replace(res, @"\*([^>]+)\*|_([^>]+)_", "$1$2");
            // remove titles
            res = Regex.Replace(res, @"#{1,6}\s*(\w*)", "$1");
            return res;
        }
    }
}