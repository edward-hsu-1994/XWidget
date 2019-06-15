using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace XWidget.ObjectHook {
    internal class HookMethodInfo {
        public MethodType Type { get; set; }
        public MethodInfo Method { get; set; }
    }

    /// <summary>
    /// 方法類型
    /// </summary>
    internal enum MethodType {
        /// <summary>
        /// 一般
        /// </summary>
        Default,
        /// <summary>
        /// 屬性設定方法
        /// </summary>
        PropertySetter,
        /// <summary>
        /// 屬性取得方法
        /// </summary>
        PropertyGatter,
        /// <summary>
        /// 索引設定方法
        /// </summary>
        IndexerSetter,
        /// <summary>
        /// 索引取得方法
        /// </summary>
        IndexerGatter
    }
}
