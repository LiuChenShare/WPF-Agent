using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace Titanium.Web.Proxy.Examples.Basic
{
    public class Proxy2
    {
        private readonly ProxyServer proxyServer;

        private readonly IDictionary<Guid, HeaderCollection> requestHeaderHistory = new ConcurrentDictionary<Guid, HeaderCollection>();

        private readonly IDictionary<Guid, HeaderCollection> responseHeaderHistory = new ConcurrentDictionary<Guid, HeaderCollection>();

        public Proxy2()
        {
            proxyServer = new ProxyServer(true);
            proxyServer.ForwardToUpstreamGateway = true;
        }

        public void Start()
        {
            proxyServer.BeforeRequest += OnRequest;
            proxyServer.BeforeResponse += OnResponse;

            var transparentProxyEndPoint = new TransparentProxyEndPoint(IPAddress.Loopback, 8003, false)
            {

            };

            proxyServer.AddEndPoint(transparentProxyEndPoint);
            proxyServer.Start();
        }

        public void Stop()
        {
            proxyServer.BeforeRequest -= OnRequest;
            proxyServer.BeforeResponse -= OnResponse;
            proxyServer.Stop();
        }

        public async Task OnRequest(object sender, SessionEventArgs e)
        {
            var requestUri = e.WebSession.Request.RequestUri;
            var modifiedUri = new UriBuilder("http", "127.0.0.1", 8888, requestUri.AbsolutePath);
            //var modifiedUri = new UriBuilder("http", "proxy.uku.im", 443, requestUri.AbsolutePath);

            Console.WriteLine("Request"+ modifiedUri.Uri);

            e.WebSession.Request.RequestUri = modifiedUri.Uri;

            requestHeaderHistory[e.Id] = e.WebSession.Request.Headers;

            if (e.WebSession.Request.HasBody)
            {
                var bodyBytes = await e.GetRequestBody();
                e.SetRequestBody(bodyBytes);

                string bodyString = await e.GetRequestBodyAsString();
                e.SetRequestBodyString(bodyString);
            }

        }

        public async Task OnResponse(object sender, SessionEventArgs e)
        {
            responseHeaderHistory[e.Id] = e.WebSession.Response.Headers;

            if (e.WebSession.Request.Method == "GET" || e.WebSession.Request.Method == "POST")
            {
                if (e.WebSession.Response.StatusCode == (int)HttpStatusCode.OK)
                {
                    if (e.WebSession.Response.ContentType != null && e.WebSession.Response.ContentType.Trim().ToLower().Contains("text/html"))
                    {
                        var bodyBytes = await e.GetResponseBody();
                        e.SetResponseBody(bodyBytes);

                        string body = await e.GetResponseBodyAsString();
                        e.SetResponseBodyString(body);
                    }
                }
            }
        }
    }
}
