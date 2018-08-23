#region Using Directives

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Xml.XPath;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

#endregion

namespace ReaService.Orleans.Api.Middleware
{
    public static class SwaggerExtensions
    {
        public static void IncludeEmbeddedXmlComments(this SwaggerGenOptions options, string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException(nameof(resourceName), "The full name of the Swagger Docs resource is required.");
            options.IncludeXmlComments(() =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new MissingManifestResourceException($"Could not find an embedded resource named '{resourceName}'. Make sure the resource name is the name of the document prepended with the namespace. For example, `Company.App.Resources.SwaggerDocs.xml`");

                    using (var reader = new StreamReader(stream))
                    {
                        return new XPathDocument(reader);
                    }
                }
            });
        }

        public static void UseLowerCasedPaths(this SwaggerOptions options)
        {
            options.PreSerializeFilters.Add((document, request) =>
            {
                var paths = document.Paths.ToDictionary(item => item.Key.ToLowerInvariantWithTemplate(), item => item.Value);
                document.Paths.Clear();
                foreach (var pathItem in paths)
                    document.Paths.Add(pathItem.Key, pathItem.Value);
            });
        }
    }

    public static class StringExtensions
    {
        /// <summary>
        ///     Returns a copy of this string converted to lowercase, skipping letters that fall between the
        ///     template indicators.
        /// </summary>
        /// <param name="s">The string to convert. This method is available on the string as an extension method.</param>
        /// <param name="templateStart">The char that indicates the start of a template. The default is '{'</param>
        /// <param name="templateEnd">The char that indicates the end of a template. The default is '}'</param>
        /// <returns>A string in lowercase except for the templates.</returns>
        public static string ToLowerInvariantWithTemplate(this string s, char templateStart = '{', char templateEnd = '}')
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            var builder = new StringBuilder();
            var skipping = false;

            for (var index = 0; index < s.Length; index++)
            {
                var c = s[index];

                if (skipping)
                {
                    if (c == templateEnd)
                        skipping = false;

                    builder.Append(c);
                    continue;
                }

                if (c == templateStart)
                {
                    skipping = true;

                    builder.Append(c);
                    continue;
                }

                builder.Append(char.IsUpper(c) ? char.ToLower(c, CultureInfo.InvariantCulture) : c);
            }

            return builder.ToString();
        }
    }
}