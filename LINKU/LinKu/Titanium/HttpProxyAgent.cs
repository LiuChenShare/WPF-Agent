using LinKu.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;

namespace LinKu.Titanium
{
    public class HttpProxyAgent
    {
        private readonly ProxyServer proxyServer;

        private int lastSessionNumber;

        protected WindowStarter MainWindow;

        public ObservableCollection<SessionListItem> Sessions
        {
            get { return MainWindow.Sessions; }
        }

        public SessionListItem SelectedSession
        {
            get => MainWindow.SelectedSession;
            set => MainWindow.SelectedSession = value;
        }

        public Dictionary<HttpWebClient, SessionListItem> sessionDictionary
        {
            get { return MainWindow.sessionDictionary; }
        }


        public static readonly DependencyProperty ClientConnectionCountProperty = DependencyProperty.Register(
            nameof(ClientConnectionCount), typeof(int), typeof(WindowStarter), new PropertyMetadata(default(int)));

        public int ClientConnectionCount
        {
            get => (int)MainWindow.GetValue(ClientConnectionCountProperty);
            set => MainWindow.SetValue(ClientConnectionCountProperty, value);
        }

        public static readonly DependencyProperty ServerConnectionCountProperty = DependencyProperty.Register(
            nameof(ServerConnectionCount), typeof(int), typeof(WindowStarter), new PropertyMetadata(default(int)));

        public int ServerConnectionCount
        {
            get => (int)MainWindow.GetValue(ServerConnectionCountProperty);
            set => MainWindow.SetValue(ServerConnectionCountProperty, value);
        }


        public HttpProxyAgent(WindowStarter mainWindow)
        {
            MainWindow = mainWindow;

            proxyServer = new ProxyServer();

            //locally trust root certificate used by this proxy 
            proxyServer.CertificateManager.TrustRootCertificate(true);


            proxyServer.ForwardToUpstreamGateway = true;

            var explicitEndPoint = ProxyFactory.GetExplictProxyEndPoint();

            proxyServer.AddEndPoint(explicitEndPoint);

            proxyServer.GetCustomUpStreamProxyFunc += ProxyFactory.GetCustomUpStreamProxyFunc;


            proxyServer.BeforeRequest += ProxyServer_BeforeRequest;
            proxyServer.BeforeResponse += ProxyServer_BeforeResponse;
            proxyServer.AfterResponse += ProxyServer_AfterResponse;
            explicitEndPoint.BeforeTunnelConnectRequest += ProxyServer_BeforeTunnelConnectRequest;
            explicitEndPoint.BeforeTunnelConnectResponse += ProxyServer_BeforeTunnelConnectResponse;
            proxyServer.ClientConnectionCountChanged += delegate { MainWindow.Dispatcher.Invoke(() => { ClientConnectionCount = proxyServer.ClientConnectionCount; }); };
            proxyServer.ServerConnectionCountChanged += delegate { MainWindow.Dispatcher.Invoke(() => { ServerConnectionCount = proxyServer.ServerConnectionCount; }); };
            proxyServer.Start();

            proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);
        }

        public void HttpProxyAgentEnd()
        {
            proxyServer.Stop();
        }

        private async Task ProxyServer_BeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            Console.WriteLine("tunnel");
            string hostname = e.WebSession.Request.RequestUri.Host;
            if (hostname.EndsWith("webex.com"))
            {
                e.Excluded = true;
            }

            await MainWindow.Dispatcher.InvokeAsync(() =>
            {
                AddSession(e);
            });
        }

        private async Task ProxyServer_BeforeTunnelConnectResponse(object sender, SessionEventArgs e)
        {
            await MainWindow.Dispatcher.InvokeAsync(() =>
            {
                if (sessionDictionary.TryGetValue(e.WebSession, out var item))
                {
                    item.Update();
                }
            });
        }

        private async Task ProxyServer_BeforeRequest(object sender, SessionEventArgs e)
        {
            SessionListItem item = null;
            await MainWindow.Dispatcher.InvokeAsync(() =>
            {
                item = AddSession(e);
            });

            if (e.WebSession.Request.HasBody)
            {
                e.WebSession.Request.KeepBody = true;
                await e.GetRequestBody();
            }
        }


        private async Task ProxyServer_BeforeResponse(object sender, SessionEventArgs e)
        {
            SessionListItem item = null;
            await MainWindow.Dispatcher.InvokeAsync(() =>
            {
                if (sessionDictionary.TryGetValue(e.WebSession, out item))
                {
                    item.Update();
                }
            });

            if (item != null)
            {
                if (e.WebSession.Response.HasBody)
                {
                    e.WebSession.Response.KeepBody = true;
                    await e.GetResponseBody();

                    await MainWindow.Dispatcher.InvokeAsync(() =>
                    {
                        item.Update();
                    });
                }
            }
        }

        private async Task ProxyServer_AfterResponse(object sender, SessionEventArgs e)
        {
            await MainWindow.Dispatcher.InvokeAsync(() =>
            {
                if (sessionDictionary.TryGetValue(e.WebSession, out var item))
                {
                    item.Exception = e.Exception;
                }
            });
        }

        private SessionListItem AddSession(SessionEventArgs e)
        {
            var item = CreateSessionListItem(e);
            Sessions.Add(item);
            sessionDictionary.Add(e.WebSession, item);
            return item;
        }

        private SessionListItem CreateSessionListItem(SessionEventArgs e)
        {
            lastSessionNumber++;
            bool isTunnelConnect = e is TunnelConnectSessionEventArgs;
            var item = new SessionListItem
            {
                Number = lastSessionNumber,
                WebSession = e.WebSession,
                IsTunnelConnect = isTunnelConnect,
            };

            if (isTunnelConnect || e.WebSession.Request.UpgradeToWebSocket)
            {
                e.DataReceived += (sender, args) =>
                {
                    var session = (SessionEventArgs)sender;
                    if (sessionDictionary.TryGetValue(session.WebSession, out var li))
                    {
                        li.ReceivedDataCount += args.Count;
                    }
                };

                e.DataSent += (sender, args) =>
                {
                    var session = (SessionEventArgs)sender;
                    if (sessionDictionary.TryGetValue(session.WebSession, out var li))
                    {
                        li.SentDataCount += args.Count;
                    }
                };
            }

            item.Update();
            return item;
        }
        
    }
}
