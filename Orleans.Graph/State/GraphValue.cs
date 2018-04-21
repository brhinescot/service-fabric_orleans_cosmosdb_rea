#region Using Directives

using System;
using System.Globalization;

#endregion

namespace Orleans.Graph.State
{
    public class GraphValue
    {
        public static implicit operator string(GraphValue x) => x?.ToString();
        public static implicit operator GraphValue(string x) => new GraphValue(x, true, x?.GetType());

        public static implicit operator DateTime(GraphValue x) => x == null ? DateTime.MinValue : DateTime.Parse(x.ToString(), DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal);
        public static implicit operator GraphValue(DateTime x) => new GraphValue($"{x.ToUniversalTime():O}", true, x.GetType());
        
        public static implicit operator Guid(GraphValue x) => Guid.ParseExact(x.ToString(), "D");
        public static implicit operator GraphValue(Guid x) => new GraphValue($"{x:D}", true, x.GetType());

        public static implicit operator bool(GraphValue x) => bool.Parse(x.ToString().UppercaseFirst());
        public static implicit operator GraphValue(bool x) => new GraphValue(x.ToString().LowercaseFirst(), false, x.GetType());
        
        public static implicit operator short(GraphValue x) => short.Parse(x.ToString());
        public static implicit operator GraphValue(short x) => new GraphValue(x.ToString(), false, x.GetType());
        
        public static implicit operator int(GraphValue x) => int.Parse(x.ToString());
        public static implicit operator GraphValue(int x) => new GraphValue(x.ToString(), false, x.GetType());
        
        public static implicit operator long(GraphValue x) => long.Parse(x.ToString());
        public static implicit operator GraphValue(long x) => new GraphValue(x.ToString(), false, x.GetType());
        
        public static implicit operator double(GraphValue x) => double.Parse(x.ToString());
        public static implicit operator GraphValue(double x) => new GraphValue(x.ToString("R", CultureInfo.InvariantCulture), false, x.GetType());
        
        public static implicit operator float(GraphValue x) => float.Parse(x.ToString());
        public static implicit operator GraphValue(float x) => new GraphValue(x.ToString("R", CultureInfo.InvariantCulture), false, x.GetType());
        
        public static implicit operator decimal(GraphValue x) => decimal.Parse(x.ToString());
        public static implicit operator GraphValue(decimal x) => new GraphValue(x.ToString("G", CultureInfo.InvariantCulture), false, x.GetType());

        private readonly string stringValue;
        private readonly bool isString;
        public Type Type { get; }

        private GraphValue(string stringValue, bool isString, Type type = null)
        {
            Type = type;
            this.isString = isString;
            this.stringValue = stringValue;

            if (string.IsNullOrEmpty(this.stringValue))
                return;
            
            this.stringValue = this.isString 
                ? stringValue[0] == '\'' 
                    ? stringValue.Substring(1, stringValue.Length - 2)
                    : stringValue 
                : stringValue;
        }

        public override string ToString()
        {
            return stringValue;
        }

        public string ToGraphString()
        {
            return isString ? $"'{stringValue}'" : stringValue;
        }

        public TEnum ToEnum<TEnum>()
        {
            return (TEnum) Enum.Parse(typeof(TEnum), stringValue);
        }
    }

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