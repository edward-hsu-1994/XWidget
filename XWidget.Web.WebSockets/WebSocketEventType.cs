namespace XWidget.Web.WebSockets {
    /// <summary>
    /// WebSocket事件
    /// </summary>
    public enum WebSocketEventType {
        /// <summary>
        /// 連線
        /// </summary>   
        Connect,

        /// <summary>
        /// 斷線
        /// </summary> 
        Disconnected,

        /// <summary>
        /// 接收訊息中
        /// </summary> 
        Receiving,

        /// <summary>
        /// 接收到訊息
        /// </summary> 
        Received
    }
}