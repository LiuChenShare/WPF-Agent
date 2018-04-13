using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace Titanium.Web.Proxy.Examples.Wpf
{
    public class ProxyFactory
    {
        private static ExternalProxy _externalProxy;

        private static ExternalProxy _getExternalProxy()
        {
            if (_externalProxy != null)
                return _externalProxy;

            _externalProxy = new ExternalProxy
            {
                HostName = "127.0.0.1",
                Port = 8888,
            };
            
            return _externalProxy;
        }

        public static ExplicitProxyEndPoint GetExplictProxyEndPoint()
        {
            return new ExplicitProxyEndPoint(IPAddress.Any, 8000, true);
        }

        public async static Task<ExternalProxy> GetCustomUpStreamProxyFunc(SessionEventArgs args)
        {
            Rules _rules = Rules.GetRules();
            DomainMethod domain = new DomainMethod();
            var AdoptRules = domain.GetAdoptRules(args.WebSession.Request.RequestUri.Host.GetRootHost(), args.WebSession.Request.RequestUri.AbsolutePath, _rules.DomainRules)?.RexStr;
            //if (args.WebSession.Request.Url.Contains("google.com"))
            //{
            //    args.IsUseExternalProxy = true;
            //}
            if (AdoptRules!=null && AdoptRules.Count > 0)
            {
                args.IsUseExternalProxy = true;
            }
            else
            {
                args.IsUseExternalProxy = false;
            }

            // return proxy
            if (args.IsUseExternalProxy)
            {
                return _getExternalProxy();
            }
            else
            {
                return null;
            }
        }
    }
}
