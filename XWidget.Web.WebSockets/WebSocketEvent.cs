using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;

namespace XWidget.Web.WebSockets {
    /// <summary>
    /// WebSocket事件
    /// </summary>
    public class WebSocketEvent {
        /// <summary>
        /// 事件類型
        /// </summary>
        public WebSocketEventType Type { get; set; }

        /// <summary>
        /// 本次事件的訊息WebSocket原始類型
        /// </summary>
        public WebSocketMessageType? MessageType { get; set; }

        /// <summary>
        /// 本次事件的接收資料
        /// </summary>
        public ArraySegment<byte> ReceivedData { get; set; }

        /// <summary>
        /// 本次連線的HttpContext
        /// </summary>
        public HttpContext Context { get; set; }

        /// <summary>
        /// 引動事件的WebSocket物件
        /// </summary>
        public WebSocket WebSocket { get; internal set; }
    }
}