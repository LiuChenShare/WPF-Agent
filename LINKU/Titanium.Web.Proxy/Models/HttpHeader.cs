﻿using System;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.Helpers;
using Titanium.Web.Proxy.Http;

namespace Titanium.Web.Proxy.Models
{
    /// <summary>
    /// Http Header object used by proxy
    /// </summary>
    public class HttpHeader
    {
        internal static readonly Version VersionUnknown = new Version(0, 0);

        internal static readonly Version Version10 = new Version(1, 0);

        internal static readonly Version Version11 = new Version(1, 1);

        internal static HttpHeader ProxyConnectionKeepAlive = new HttpHeader("Proxy-Connection", "keep-alive");

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="Exception"></exception>
        public HttpHeader(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Name cannot be null");
            }

            Name = name.Trim();
            Value = value.Trim();
        }

        /// <summary>
        /// Header Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Header Value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns header as a valid header string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}: {Value}";
        }

        internal static HttpHeader GetProxyAuthorizationHeader(string userName, string password)
        {
            var result = new HttpHeader(KnownHeaders.ProxyAuthorization,
                "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}")));
            return result;
        }

        internal async Task WriteToStreamAsync(HttpWriter writer)
        {
            await writer.WriteAsync(Name);
            await writer.WriteAsync(": ");
            await writer.WriteLineAsync(Value);
        }
    }
}
