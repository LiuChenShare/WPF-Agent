using System;
using System.Collections;
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

        /// <summary>
        /// 获取符合的规则
        /// </summary>
        /// <param name="host"></param>
        /// <param name="url"></param>
        /// <param name="Rules">规则</param>
        /// <param name="wildcardRules">带通配符的规则</param>
        /// <returns></returns>
        public DomainRules GetDomainRules(string host, string url, Hashtable Rules, List<DomainRules> wildcardRules)
        {
            var rexStr = new List<string>();
            var blockadeRules = new DomainRules()
            {
                Host = host,
                RexStr = new List<string>()
            };
            if (Rules.Contains(host))
            {
                rexStr = (List<string>)Rules[host];
            }
            else
            {
                foreach (var rules in wildcardRules)
                {
                    if (host.Length >= rules.Host.Length)
                    {
                        string str = host.Substring(host.Length - rules.Host.Length);
                        if (string.Equals(str, rules.Host, StringComparison.OrdinalIgnoreCase))
                        {
                            blockadeRules.Host = "*" + rules.Host;
                            rexStr = rules.RexStr;
                        }
                    }
                    else if (host.Length == rules.Host.Length - 1 && rules.Host.StartsWith(".") && rules.Host.Length >= 2)
                    {
                        //这里是为了在配置rule时配置的"*.iqiyi.com"可以同时支持"XXXX.iqiyi.com"和"iqiyi.com"
                        if (string.Equals(host, rules.Host.Substring(1), StringComparison.OrdinalIgnoreCase))
                        {
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

        /// <summary>
        /// 将规则集合转换为区分通配符的模型数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static RulesResult ToRulesResult(this List<DomainRules> list)
        {
            RulesResult model = new RulesResult();
            var wildcard = list.Where(x => x.Host.StartsWith("*")).Select(x => x.ToWildcardRules()).ToList();
            model.Rules = list;
            model.WildcardRules = wildcard;
            return model;
        }
    }
}
