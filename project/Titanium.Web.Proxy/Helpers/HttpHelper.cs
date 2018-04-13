﻿using StreamExtended.Network;
using System;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.Extensions;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Shared;

namespace Titanium.Web.Proxy.Helpers
{
    internal static class HttpHelper
    {
        private static readonly Encoding defaultEncoding = Encoding.GetEncoding("ISO-8859-1");

        /// <summary>
        /// Gets the character encoding of request/response from content-type header
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        internal static Encoding GetEncodingFromContentType(string contentType)
        {
            try
            {
                //return default if not specified
                if (contentType == null)
                {
                    return defaultEncoding;
                }

                //extract the encoding by finding the charset
                var parameters = contentType.Split(ProxyConstants.SemiColonSplit);
                foreach (string parameter in parameters)
                {
                    var split = parameter.Split(ProxyConstants.EqualSplit, 2);
                    if (split.Length == 2 && split[0].Trim().EqualsIgnoreCase(KnownHeaders.ContentTypeCharset))
                    {
                        string value = split[1];
                        if (value.Equals("x-user-defined", StringComparison.OrdinalIgnoreCase))
                        {
                            //todo: what is this?
                            continue;
                        }

                        if (value.Length > 2 && value[0] == '"' && value[value.Length - 1] == '"')
                        {
                            value = value.Substring(1, value.Length - 2);
                        }

                        return Encoding.GetEncoding(value);
                    }
                }
            }
            catch
            {
                //parsing errors
                // ignored
            }

            //return default if not specified
            return defaultEncoding;
        }

        internal static string GetBoundaryFromContentType(string contentType)
        {
            if (contentType != null)
            {
                //extract the boundary
                var parameters = contentType.Split(ProxyConstants.SemiColonSplit);
                foreach (string parameter in parameters)
                {
                    var split = parameter.Split(ProxyConstants.EqualSplit, 2);
                    if (split.Length == 2 && split[0].Trim().EqualsIgnoreCase(KnownHeaders.ContentTypeBoundary))
                    {
                        string value = split[1];
                        if (value.Length > 2 && value[0] == '"' && value[value.Length - 1] == '"')
                        {
                            value = value.Substring(1, value.Length - 2);
                        }

                        return value;
                    }
                }
            }

            //return null if not specified
            return null;
        }

        /// <summary>
        /// Tries to get root domain from a given hostname
        /// Adapted from below answer
        /// https://stackoverflow.com/questions/16473838/get-domain-name-of-a-url-in-c-sharp-net
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        internal static string GetWildCardDomainName(string hostname)
        {
            //only for subdomains we need wild card
            //example www.google.com or gstatic.google.com
            //but NOT for google.com
            if (hostname.Split(ProxyConstants.DotSplit).Length > 2)
            {
                int idx = hostname.IndexOf(ProxyConstants.DotSplit);

                //issue #352
                if(hostname.Substring(0, idx).Contains("-"))
                {
                    return hostname;
                }

                string rootDomain = hostname.Substring(idx + 1);
                return "*." + rootDomain;
            }

            //return as it is
            return hostname;
        }

        /// <summary>
        /// Determines whether is connect method.
        /// </summary>
        /// <param name="clientStream">The client stream.</param>
        /// <returns>1: when CONNECT, 0: when valid HTTP method, -1: otherwise</returns>
        internal static async Task<int> IsConnectMethod(CustomBufferedStream clientStream)
        {
            bool isConnect = true;
            int legthToCheck = 10;
            for (int i = 0; i < legthToCheck; i++)
            {
                int b = await clientStream.PeekByteAsync(i);
                if (b == -1)
                {
                    return -1;
                }

                if (b == ' ' && i > 2)
                {
                    // at least 3 letters and a space
                    return isConnect ? 1 : 0;
                }

                char ch = (char)b;
                if (!char.IsLetter(ch))
                {
                    // non letter or too short
                    return -1;
                }

                if (i > 6 || ch != "CONNECT"[i])
                {
                    isConnect = false;
                }
            }

            // only letters
            return isConnect ? 1 : 0;
        }
    }
}
