using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace Titanium.Web.Proxy.Examples.Wpf
{
    public class HttpProxyAgent
    {
        private readonly ProxyServer proxyServer;

        private int lastSessionNumber;

        protected MainWindow MainWindow;

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
            nameof(ClientConnectionCount), typeof(int), typeof(MainWindow), new PropertyMetadata(default(int)));

        public int ClientConnectionCount
        {
            get => (int)MainWindow.GetValue(ClientConnectionCountProperty);
            set => MainWindow.SetValue(ClientConnectionCountProperty, value);
        }

        public static readonly DependencyProperty ServerConnectionCountProperty = DependencyProperty.Register(
            nameof(ServerConnectionCount), typeof(int), typeof(MainWindow), new PropertyMetadata(default(int)));

        public int ServerConnectionCount
        {
            get => (int)MainWindow.GetValue(ServerConnectionCountProperty);
            set => MainWindow.SetValue(ServerConnectionCountProperty, value);
        }


        public HttpProxyAgent(MainWindow mainWindow)
        {
            MainWindow = mainWindow;

            MainWindow.OnSelectedSessionChanged += () =>
            {
                _onMainWindowSelectSessionChanged();
                _onMainWindowRule(mainWindow._rules.DomainRules);
            };

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


        private void _onMainWindowSelectSessionChanged()
        {
            if (SelectedSession == null)
            {
                return;
            }

            const int truncateLimit = 1024;

            var session = SelectedSession.WebSession;
            var request = session.Request;
            var data = (request.IsBodyRead ? request.Body : null) ?? new byte[0];
            bool truncated = data.Length > truncateLimit;
            if (truncated)
            {
                data = data.Take(truncateLimit).ToArray();
            }

            //string hexStr = string.Join(" ", data.Select(x => x.ToString("X2")));
            var sb = new StringBuilder();
            sb.Append(request.HeaderText);
            sb.Append(request.Encoding.GetString(data));
            sb.Append(truncated ? Environment.NewLine + $"Data is truncated after {truncateLimit} bytes" : null);
            //sb.Append((request as ConnectRequest)?.ClientHelloInfo);
            MainWindow.TextBoxRequest.Text = sb.ToString();

            var response = session.Response;
            data = (response.IsBodyRead ? response.Body : null) ?? new byte[0];
            truncated = data.Length > truncateLimit;
            if (truncated)
            {
                data = data.Take(truncateLimit).ToArray();
            }

            //hexStr = string.Join(" ", data.Select(x => x.ToString("X2")));
            sb = new StringBuilder();
            sb.Append(response.HeaderText);
            sb.Append(response.Encoding.GetString(data));
            sb.Append(truncated ? Environment.NewLine + $"Data is truncated after {truncateLimit} bytes" : null);
            //sb.Append((response as ConnectResponse)?.ServerHelloInfo);
            if (SelectedSession.Exception != null)
            {
                sb.Append(Environment.NewLine);
                sb.Append(SelectedSession.Exception);
            }

            MainWindow.TextBoxResponse.Text = sb.ToString();
        }

        private void _onMainWindowRule(List<DomainRules> domainRules)
        {
            if (SelectedSession == null)
            {
                return;
            }
            DomainMethod domain = new DomainMethod();

            var rootHost = SelectedSession.Host.GetRootHost();

            var AdoptRules = domain.GetAdoptRules(rootHost, SelectedSession.Url, domainRules)?.RexStr;
            var Rules = domainRules.Where(x => x.Host == rootHost).FirstOrDefault()?.RexStr;

            MainWindow.Host.Content = rootHost;
            MainWindow.AdoptRules.Text = null;
            MainWindow.Rule.Text = null;
            if (AdoptRules != null && AdoptRules.Count != 0)
            {
                var adoptRulesStr = String.Join("\n", AdoptRules.ToArray());
                //MainWindow.AdoptRules.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(adoptRulesStr));
                MainWindow.AdoptRules.Text = "通过验证的正则为：\n" + adoptRulesStr;
            }
            if (Rules != null)
            {
                var rulesStr = String.Join("\n", Rules.ToArray());
                //MainWindow.Rules.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(rulesStr));
                MainWindow.Rule.Text = rulesStr;
            }
            
        }
    }
}
