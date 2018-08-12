XWidget.Web.Mvc
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.Web.Mvc.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Web.Mvc/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.Web.Mvc.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Web.Mvc/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

ASP.net Core控制器與分頁結果相關擴充並支援`XWidget.Web.Exceptions`套件的例外處理

## Controller的擴充類型: ControllerBase
1. 提供`OnUnknowException`方法提供複寫
2. 提供`Paging`方法將列舉結果轉換為分頁結果