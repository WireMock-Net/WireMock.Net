using System;
using JetBrains.Annotations;
#if !NETSTANDARD
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace WireMock.Util
{
    internal static class UriUtils
    {
        public static Uri CreateUri([NotNull] Uri uri, PathString pathBase)
        {
            if (!pathBase.HasValue)
            {
                return uri;
            }

            var builder = new UriBuilder(uri);
            builder.Path = RemoveFirst(builder.Path, pathBase.Value);

            return builder.Uri;
        }

        private static string RemoveFirst(string text, string search)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }

            return text.Substring(0, pos) + text.Substring(pos + search.Length);
        }
    }
}
