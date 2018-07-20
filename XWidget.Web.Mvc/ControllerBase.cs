using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XWidget.Web.Exceptions;

namespace XWidget.Web.Mvc {
    public abstract class ControllerBase : Controller {
        /// <summary>
        /// 當發生未知例外
        /// </summary>
        /// <param name="exception">例外</param>
        public virtual void OnUnknowException(ActionExecutedContext context) {
            var unknow = new UnknowException();
            context.Result = new JsonResult(unknow);
            context.HttpContext.Response.StatusCode = unknow.StatusCode;
            context.ExceptionHandled = true;
        }

        public override void OnActionExecuted(ActionExecutedContext context) {
            if (context.Exception != null) {
                if (context.Exception is ExceptionBase error) {
                    context.Result = new JsonResult(error);
                    context.HttpContext.Response.StatusCode = error.StatusCode;
                    context.ExceptionHandled = true;
                } else {
                    OnUnknowException(context);
                }
            }
            base.OnActionExecuted(context);
        }

        /// <summary>
        /// 產生分頁結果
        /// </summary>
        /// <typeparam name="T">分頁元素類型</typeparam>
        /// <param name="result">結果集合</param>
        /// <param name="skip">起始索引</param>
        /// <param name="take">取得筆數</param>
        /// <returns>分頁結果</returns>
        public PaginationResult<IEnumerable<T>> Paging<T>(IEnumerable<T> result, int skip, int take) {
            return new PaginationResult<IEnumerable<T>>() {
                Skip = skip,
                Take = take,
                Result = result.Skip(skip).Take(take),
                TotalCount = result.Count()
            };
        }
    }
}
