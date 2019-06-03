using System;
using System.Reactive.Subjects;

namespace XWidget.Web.WebSockets {
    /// <summary>
    /// WebSocket事件處理器基礎類別
    /// </summary>
    public abstract class WebSocketHandlerBase {
        /// <summary>
        /// 事件訂閱
        /// </summary>
        public Subject<WebSocketEvent> Events { get; private set; }
            = new Subject<WebSocketEvent>();
    }
}
