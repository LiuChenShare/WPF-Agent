using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using System.Configuration;

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
                HostName = ConfigurationManager.AppSettings["HostName"] ?? "127.0.0.1",
                Port = int.Parse(ConfigurationManager.AppSettings["Port"] ?? "8888"),
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
            bool adopt = false;

            var blockadeRules = domain.GetBlockadeRules(args.WebSession.Request.RequestUri.Host, args.WebSession.Request.RequestUri.AbsolutePath);
            if (blockadeRules.RexStr != null && blockadeRules.RexStr.Count > 0)
            {
                //说明进了黑名单匹配
            }
            else
            {
                var adoptRules = domain.GetAdoptRules(args.WebSession.Request.RequestUri.Host, args.WebSession.Request.RequestUri.AbsolutePath);
                if (adoptRules.RexStr != null && adoptRules.RexStr.Count > 0)
                {
                    //说明通过了白名单匹配
                    adopt = true;
                }
            }

            //if (args.WebSession.Request.Url.Contains("google.com"))
            //{
            //    args.IsUseExternalProxy = true;
            //}

            args.IsUseExternalProxy = adopt;

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
