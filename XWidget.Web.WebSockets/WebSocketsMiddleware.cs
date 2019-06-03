using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XWidget.Web.WebSockets {
    /// <summary>
    /// WebSocket中間層
    /// </summary>
    /// <typeparam name="THandler">處理類型</typeparam>
    public class WebSocketsMiddleware<THandler>
        where THandler : WebSocketHandlerBase {
        /// <summary>
        /// Middleware的下一個管線流程
        /// </summary>
        private readonly RequestDelegate Next;

        /// <summary>
        /// WebSocket設定選項
        /// </summary>
        private readonly WebSocketOptions Options;

        /// <summary>
        /// 預設的接收訊息緩衝區大小
        /// </summary>
        private const int _defaultBufferSize = 8 * 1024;

        /// <summary>
        /// Middleware建構子
        /// </summary>
        public WebSocketsMiddleware(RequestDelegate next, WebSocketOptions options) {
            Next = next;
            Options = options;
        }

        public async Task Invoke(HttpContext context) {
            // 如果不是WebSocket請求
            if (!context.WebSockets.IsWebSocketRequest) {
                await Next(context); // 交由下個管線流程處理請求
                return;
            }

            // 取得Thandler實例，請注意THandler類型必須註冊DI
            var handler = (THandler)context.RequestServices.GetService(typeof(THandler));

            WebSocket socket = null;
            try {
                // 同意WebSocket連線並取得WebSocket物件
                socket = await context.WebSockets.AcceptWebSocketAsync();

                // 將`連線`事件丟給Handler
                handler.Events.OnNext(new WebSocketEvent() {
                    Type = WebSocketEventType.Connect,
                    Context = context,
                    WebSocket = socket
                });

                // 完整接收訊息
                var receivedSegs = new List<ArraySegment<byte>>();

                // 監聽迴圈，在WebSocket是打開的情況下持續監聽
                while (socket.State == WebSocketState.Open) {
                    WebSocketReceiveResult receiveResult;

                    #region 片段接收迴圈
                    do {
                        // 緩衝區
                        var buffer = new ArraySegment<byte>(new byte[Options?.ReceiveBufferSize ?? _defaultBufferSize]);

                        // 接收資料
                        receiveResult = await socket.ReceiveAsync(buffer, CancellationToken.None);

                        // 本次接收循環資料區段
                        var receiving = new ArraySegment<byte>(buffer.Array, 0, receiveResult.Count);

                        // 傳遞`接收中`事件
                        handler.Events.OnNext(new WebSocketEvent() {
                            Type = WebSocketEventType.Receiving,
                            MessageType = receiveResult.MessageType,
                            ReceivedData = receiving,
                            Context = context,
                            WebSocket = socket
                        });

                        // 保留本次接收區段
                        receivedSegs.Add(receiving);
                    } while (!receiveResult.EndOfMessage); // 確保本次接收片段已經結束
                    #endregion

                    #region 合併接收片段
                    var received = new byte[receivedSegs.Sum(x => x.Count)];
                    int offset = 0;
                    foreach (var seg in receivedSegs) {
                        Buffer.BlockCopy(seg.Array, 0, received, offset, seg.Count);
                        offset += seg.Count;
                    }
                    #endregion

                    // 全部片段接收完成後傳遞`接收`事件，傳遞所有片段拼裝的結果
                    handler.Events.OnNext(new WebSocketEvent() {
                        Type = WebSocketEventType.Received,
                        MessageType = receiveResult.MessageType,
                        ReceivedData = new ArraySegment<byte>(received),
                        Context = context,
                        WebSocket = socket
                    });
                }

                // 跳脫監聽迴圈表示WebSocket已經斷線，傳遞`斷線事件`
                handler.Events.OnNext(new WebSocketEvent() {
                    Type = WebSocketEventType.Disconnected,
                    Context = context,
                    WebSocket = socket
                });

                // 結束Subject
                handler.Events.OnCompleted();
            } catch (Exception e) {
                // 發生例外，傳遞事件
                handler.Events.OnError(e);

                // 如果WebSocket還連線中則關閉連線
                if (socket.State == WebSocketState.Open) {
                    await socket.CloseAsync(WebSocketCloseStatus.Empty, e.ToString(), CancellationToken.None);
                }
            }
        }
    }
}
