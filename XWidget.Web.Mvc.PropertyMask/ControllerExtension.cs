using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.AspNetCore.Mvc {
    public static class ControllerExtension {
        /// <summary>
        /// 回傳遮蔽後的JSON結果
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="data">資料</param>
        /// <param name="patternName">模式名稱</param>
        /// <param name="jsonSerializerSettings">JSON序列化設定</param>
        /// <returns>遮蔽後的JSON結果</returns>
        public static JsonResult JsonByMask<T>(
            this Controller controller,
            T data,
            string patternName = null,
            JsonSerializerSettings jsonSerializerSettings = null)
            where T : class {

            // 檢查是否有自訂序列化設定選項
            if (jsonSerializerSettings == null) {
                // 直接調用預設的設定序列化
                return controller.Json(
                    controller.Mask(data, patternName));
            } else {
                // 加入自訂序列化設定選項
                return controller.Json(
                   controller.Mask(data, patternName),
                   jsonSerializerSettings);
            }
        }

        /// <summary>
        /// 取得屏蔽過濾後的資料
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="source">原始資料</param>
        /// <param name="patternName">模式名稱</param>
        /// <returns>屏蔽過濾後的資料</returns>
        public static T Mask<T>(this Controller controller, T source, string patternName = null)
            where T : class {
            return Masker.Mask(source, controller, patternName);
        }
    }
}
