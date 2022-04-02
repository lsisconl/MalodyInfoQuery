namespace MalodyInfoQuery.Utils
{
    internal class Html
    {
        public static string EscToHtml(string input)
        {
            if (string.IsNullOrEmpty(input)) { return ""; }
 
            input = input.Replace("&#8482;", "™")
                .Replace("&reg;", "®")
                .Replace("&copy;", "©")
                .Replace("&nbsp;", " ")
                .Replace("&gt;", ">")
                .Replace("&lt;", "<")
                .Replace("&quot;", "\"")
                .Replace("&#39;", "'")
                .Replace("&amp;", "&");
            return input;
        }
    }
}