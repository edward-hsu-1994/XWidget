using Newtonsoft.Json;
using System;

namespace XWidget.Web.Exceptions {
    /// <summary>
    /// 訂製例外基礎類別
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ExceptionBase : Exception {
        /// <summary>
        /// 錯誤代碼
        /// </summary>
        [JsonProperty]
        public int Code { get; private set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }

        /// <summary>
        /// 訊息
        /// </summary>
        [JsonProperty]
        public new string Message { get; set; }

        /// <summary>
        /// Http狀態碼
        /// </summary>
        public int StatusCode { get; private set; } = 500;

        /// <summary>
        /// 訂製例外基礎類別的建構子
        /// </summary>
        /// <param name="code">錯誤代碼</param>
        /// <param name="name">名稱</param>
        /// <param name="message">訊息</param>
        public ExceptionBase(int code, string name, string message) {
            Code = code;
            Name = name;
            Message = message;
        }

        /// <summary>
        /// 訂製例外基礎類別的建構子
        /// </summary>
        /// <param name="statusCode">Http狀態碼</param>
        /// <param name="code">錯誤代碼</param>
        /// <param name="name">名稱</param>
        /// <param name="message">訊息</param>
        public ExceptionBase(int statusCode, int code, string name, string message) {
			StatusCode = statusCode;
            Code = code;
            Name = name;
            Message = message;
        }
    }
}
