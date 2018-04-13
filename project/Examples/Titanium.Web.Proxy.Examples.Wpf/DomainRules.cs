using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Titanium.Web.Proxy.Examples.Wpf
{
    /// <summary>
    /// 域名匹配规则
    /// </summary>
    public class DomainRules
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 正则
        /// </summary>
        public List<string> RexStr { get; set; }

    }

    /// <summary>
    /// 域名匹配规则列表
    /// </summary>
    public class Rules
    {
        public List<DomainRules> DomainRules { get; set; }

        private static Rules _rules = null;

        public static Rules GetRules()
        {
            if(_rules == null)
            {
                _rules = new Rules();
                _rules.DomainRules = new List<DomainRules>();
                FileJson fileJson = new FileJson();
                var result = fileJson.GetResult();
                _rules.DomainRules = result;
            }
            return _rules;
        }
    }
}
