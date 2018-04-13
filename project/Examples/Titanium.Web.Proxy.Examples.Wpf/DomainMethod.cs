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
        /// <param name="domainRulesList"></param>
        /// <returns></returns>
        public DomainRules GetAdoptRules(string host, string url, List<DomainRules> domainRulesList)
        {
            //var RexStrList = domainRulesList?.Where(x => host.Contains(x.Host)).Select(x => x.RexStr).ToList();
            var RexStrList = domainRulesList?.Where(x => x.Host == host).ToList();
            var Rules = new DomainRules
            {
                Host = host,
                RexStr = new List<string>()
            };
            if (RexStrList.Count > 0)
            {
                //Rules.Host = RexStrList.FirstOrDefault().Host;
                foreach (var pattern in RexStrList.FirstOrDefault().RexStr)
                {
                    if (Regex.IsMatch(url, pattern))
                    {
                        //正则通过，把符合的正则存下来
                        Rules.RexStr.Add(pattern);
                    }
                    //if (url.Contains(pattern))
                    //{
                    //    Rules.RexStr.Add(pattern);
                    //}
                }
            }
            return Rules;
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
}
