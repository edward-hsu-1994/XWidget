using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.EFLogic {
    /// <summary>
    /// 連續刪除制止器
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RemoveCascadeStopperAttribute : Attribute {
    }
}
