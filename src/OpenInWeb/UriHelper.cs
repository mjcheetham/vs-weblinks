using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenInWeb
{
    public static class UriHelper
    {
        public static string Decode(string encodedString)
        {
            return Uri.UnescapeDataString(encodedString).Replace('+', ' ');
        }

        public static string Encode(string rawString)
        {
            return Uri.EscapeUriString(rawString);
        }

        public static void AppendPath(UriBuilder builder, string part)
        {
            var trailing = builder.Path.EndsWith("/");
            if (trailing != part.StartsWith("/"))
            {
                builder.Path += part;
            }
            else if (trailing && part.Length > 0)
            {
                builder.Path += part.Substring(1);
            }
            else
            {
                builder.Path += "/" + part;
            }
        }

        public static string ToQueryString(IDictionary<string, string> dictionary)
        {
            var sb = new StringBuilder();

            foreach (var kvp in dictionary)
            {
                string key = Encode(kvp.Key);
                string value = Encode(kvp.Value);

                if (sb.Length > 0)
                {
                    sb.Append('&');
                }

                sb.Append(key).Append('=').Append(value);
            }

            return sb.ToString();
        }
    }
}
