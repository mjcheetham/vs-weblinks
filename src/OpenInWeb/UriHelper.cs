using System;

namespace OpenInWeb
{
    public static class UriHelper
    {
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
    }
}
