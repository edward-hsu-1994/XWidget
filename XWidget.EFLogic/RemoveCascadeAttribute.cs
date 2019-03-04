using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.EFLogic {
    /// <summary>
    /// 連鎖刪除模式
    /// </summary>
    public enum RemoveCascadeMode {
        /// <summary>
        /// 僅選擇項目不連鎖刪除
        /// </summary>
        OptOut,
        /// <summary>
        /// 僅選擇項目可連鎖
        /// </summary>
        OptIn,
    }

    /// <summary>
    /// 連鎖刪除設定
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RemoveCascadeAttribute : Attribute {
        /// <summary>
        /// 連鎖刪除模式
        /// </summary>
        public RemoveCascadeMode Mode { get; set; } = RemoveCascadeMode.OptOut;
    }
}
