using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinKu.Models
{
    public enum MsgState
    {
        Susuccess = 100,
        Error = 101,
        Wait = 102,
        Step = 103,
        /// <summary>
        /// 超时
        /// </summary>
        Overtime = 104
    }
}
