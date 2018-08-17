using System;
using JetBrains.Annotations;
using WireMock.Models;
#if !USE_ASPNETCORE
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace WireMock.Util
{
    internal static class UrlUtils
    {
        public static UrlDetails Parse([NotNull] Uri uri, PathString pathBase)
        {
            if (!pathBase.HasValue)
            {
                return new UrlDetails(uri, uri);
            }

            var builder = new UriBuilder(uri);
            builder.Path = RemoveFirst(builder.Path, pathBase.Value);

            return new UrlDetails(uri, builder.Uri);
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
