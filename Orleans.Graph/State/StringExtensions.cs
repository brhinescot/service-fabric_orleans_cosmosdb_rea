namespace Orleans.Graph.State
{
    public static class StringExtensions
    {
        public static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            if (s.Length == 1)
                return s.ToUpper();

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string LowercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            if (s.Length == 1)
                return s.ToLower();

            return char.ToLower(s[0]) + s.Substring(1);
        }
    }
}