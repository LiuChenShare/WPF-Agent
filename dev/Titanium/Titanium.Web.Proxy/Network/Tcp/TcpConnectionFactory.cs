﻿using StreamExtended.Network;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.Extensions;
using Titanium.Web.Proxy.Helpers;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace Titanium.Web.Proxy.Network.Tcp
{
    /// <summary>
    /// A class that manages Tcp Connection to server used by this proxy server
    /// </summary>
    internal class TcpConnectionFactory
    {
        /// <summary>
        ///  Creates a TCP connection to server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="remoteHostName"></param>
        /// <param name="remotePort"></param>
        /// <param name="httpVersion"></param>
        /// <param name="isHttps"></param>
        /// <param name="isConnect"></param>
        /// <param name="upStreamEndPoint"></param>
        /// <param name="externalProxy"></param>
        /// <returns></returns>
        internal async Task<TcpConnection> CreateClient(ProxyServer server, 
            string remoteHostName, int remotePort, Version httpVersion, bool isHttps,
            bool isConnect, IPEndPoint upStreamEndPoint, ExternalProxy externalProxy)
        {
            bool useUpstreamProxy = false;

            //check if external proxy is set for HTTP/HTTPS
            if (externalProxy != null && !(externalProxy.HostName == remoteHostName && externalProxy.Port == remotePort))
            {
                useUpstreamProxy = true;

                //check if we need to ByPass
                if (externalProxy.BypassLocalhost && NetworkHelper.IsLocalIpAddress(remoteHostName))
                {
                    useUpstreamProxy = false;
                }
            }

            TcpClient client = null;
            CustomBufferedStream stream = null;

            try
            {
                client = new TcpClient(upStreamEndPoint);

                //If this proxy uses another external proxy then create a tunnel request for HTTP/HTTPS connections
                if (useUpstreamProxy)
                {
                    await client.ConnectAsync(externalProxy.HostName, externalProxy.Port);
                }
                else
                {
                    await client.ConnectAsync(remoteHostName, remotePort);
                }

                stream = new CustomBufferedStream(client.GetStream(), server.BufferSize);

                if (useUpstreamProxy && (isConnect || isHttps))
                {
                    var writer = new HttpRequestWriter(stream, server.BufferSize);
                    await writer.WriteLineAsync($"CONNECT {remoteHostName}:{remotePort} HTTP/{httpVersion}");
                    await writer.WriteLineAsync($"Host: {remoteHostName}:{remotePort}");
                    await writer.WriteLineAsync($"{KnownHeaders.Connection}: {KnownHeaders.ConnectionKeepAlive}");

                    if (!string.IsNullOrEmpty(externalProxy.UserName) && externalProxy.Password != null)
                    {
                        await HttpHeader.ProxyConnectionKeepAlive.WriteToStreamAsync(writer);
                        await writer.WriteLineAsync(KnownHeaders.ProxyAuthorization + ": Basic " +
                                                    Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                                        externalProxy.UserName + ":" + externalProxy.Password)));
                    }

                    await writer.WriteLineAsync();

                    using (var reader = new CustomBinaryReader(stream, server.BufferSize))
                    {
                        string httpStatus = await reader.ReadLineAsync();

                        Response.ParseResponseLine(httpStatus, out _, out int statusCode, out string statusDescription);

                        if (statusCode != 200 && !statusDescription.EqualsIgnoreCase("OK") 
                            && !statusDescription.EqualsIgnoreCase("Connection Established"))
                        {
                            throw new Exception("Upstream proxy failed to create a secure tunnel");
                        }

                        await reader.ReadAndIgnoreAllLinesAsync();
                    }
                }

                if (isHttps)
                {
                    var sslStream = new SslStream(stream, false, server.ValidateServerCertificate, server.SelectClientCertificate);
                    stream = new CustomBufferedStream(sslStream, server.BufferSize);

                    await sslStream.AuthenticateAsClientAsync(remoteHostName, null, server.SupportedSslProtocols, server.CheckCertificateRevocation);
                }

                client.ReceiveTimeout = server.ConnectionTimeOutSeconds * 1000;
                client.SendTimeout = server.ConnectionTimeOutSeconds * 1000;
            }
            catch (Exception)
            {
                stream?.Dispose();
                client?.Close();
                throw;
            }

            return new TcpConnection(server)
            {
                UpStreamProxy = externalProxy,
                UpStreamEndPoint = upStreamEndPoint,
                HostName = remoteHostName,
                Port = remotePort,
                IsHttps = isHttps,
                UseUpstreamProxy = useUpstreamProxy,
                TcpClient = client,
                StreamReader = new CustomBinaryReader(stream, server.BufferSize),
                StreamWriter = new HttpRequestWriter(stream, server.BufferSize),
                Stream = stream,
                Version = httpVersion
            };
        }
    }
}
