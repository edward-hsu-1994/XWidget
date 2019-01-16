using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.EFLogic {
    /// <summary>
    /// 連續刪除屬性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RemoveCascadePropertyAttribute : Attribute {
    }
}
