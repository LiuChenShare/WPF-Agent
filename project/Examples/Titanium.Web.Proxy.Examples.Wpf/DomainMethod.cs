using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Titanium.Web.Proxy.Examples.Wpf
{
    public class DomainMethod
    {
        /// <summary>
        /// 根据当前配置以及请求的url获取采用的正则
        /// </summary>
        /// <param name="host"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public DomainRules GetAdoptRules(string host, string url)
        {
            var rule = Rules.GetRules();
            var rexStr = new List<string>();
            var adoptRules = new DomainRules()
            {
                Host = host,
                RexStr = new List<string>()
            };
            if (rule.WhiteRules.Contains(host))
            {
                rexStr = (List<string>)rule.WhiteRules[host];
            }
            else
            {
                foreach (var whiteWildcardRules in rule.WhiteWildcardRules)
                {
                    if (host.Length >= whiteWildcardRules.Host.Length)
                    {
                        string str = host.Substring(host.Length - whiteWildcardRules.Host.Length);
                        if (string.Equals(str, whiteWildcardRules.Host, StringComparison.OrdinalIgnoreCase))
                        {
                            adoptRules.Host = "*" + whiteWildcardRules.Host;
                            rexStr = whiteWildcardRules.RexStr;
                        }
                    }
                }
            }
            foreach (var pattern in rexStr)
            {
                if (Regex.IsMatch(url, pattern))
                {
                    //正则通过，把符合的正则存下来
                    adoptRules.RexStr.Add(pattern);
                }
                //if (url.Contains(pattern))
                //{
                //    Rules.RexStr.Add(pattern);
                //}
            }
            return adoptRules;
        }

        /// <summary>
        /// 根据当前配置以及请求的url获取黑名单的正则
        /// </summary>
        /// <param name="host"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public DomainRules GetBlockadeRules(string host, string url)
        {
            var rule = Rules.GetRules();
            var rexStr = new List<string>();
            var blockadeRules = new DomainRules()
            {
                Host = host,
                RexStr = new List<string>()
            };
            if (rule.BlackRules.Contains(host))
            {
                rexStr = (List<string>)rule.BlackRules[host];
            }
            else
            {
                foreach (var blackWildcardRules in rule.BlackWildcardRules)
                {
                    if(host.Length >= blackWildcardRules.Host.Length)
                    {
                        string str = host.Substring(host.Length - blackWildcardRules.Host.Length);
                        if (string.Equals(str, blackWildcardRules.Host, StringComparison.OrdinalIgnoreCase))
                        {
                            blockadeRules.Host = "*" + blackWildcardRules.Host;
                            rexStr = blackWildcardRules.RexStr;
                        }
                    }
                }
            }
            foreach (var pattern in rexStr)
            {
                if (Regex.IsMatch(url, pattern))
                {
                    //正则通过，把符合的正则存下来
                    blockadeRules.RexStr.Add(pattern);
                }
            }
            return blockadeRules;
        }
    }

    /// <summary>
    /// String扩展方法
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 获取根域名
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static string GetRootHost(this string host)
        {
            string[] sArray = host.Split('.');
            int a = sArray.Length;
            string rootHost = host;
            if (a >= 2)
            {
                rootHost = sArray[a - 2] + "." + sArray[a - 1];
            }
            return rootHost;
        }
    }

    /// <summary>
    /// Model扩展方法
    /// </summary>
    public static class ModelExtension
    {
        /// <summary>
        /// 获取根域名
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static DomainRules ToWildcardRules(this DomainRules rule)
        {
            DomainRules model = new DomainRules();
            if (rule != null)
            {
                model.Host = rule.Host.Substring(1);
                model.RexStr = rule.RexStr;
            }
            return model;
        }
    }
}
