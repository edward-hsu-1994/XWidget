XWidget.Web.Exceptions
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.Web.Exceptions.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Web.Exceptions/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.Web.Exceptions.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Web.Exceptions/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

ASP.net Core常用例外便於序列化的類型

## 提供的例外類型
1.ExceptionBase:
例外基底類別，提供HttpStatusCode欄位便於在`Controller`中的`OnException`用以回應

2.UnknowException:
未知的例外，系統發生未知錯誤

3.AuthorizationException:
授權無效，您目前尚未登入或您目前使用的授權失效，您可以嘗試重新登入或重新取得授權

4.PermissionsException:
權限不足，您無法進行此操作

5.NotFoundException:
找不到目標，在系統中找不到您所指定的目標資源

6.OperatorException:
操作錯誤，您的操作有誤

7.ParameterException:
參數錯誤，您輸入的參數有誤