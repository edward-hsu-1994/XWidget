XWidget.Web
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.Web.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Web/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.Web.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Web/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

ASP.net Core MVC實用方法與擴充方法

## Middleware
### NoCacheMiddleware
不快取中間層，設定Http Response快取控制為不進行快取
```csharp
app.UseNoCache();
```

## Extensions
### EnableRangeRequest
啟用RangeRequest功能，詳情可見此問題「[Partial content in .NET Core MVC](https://stackoverflow.com/questions/48711209/partial-content-in-net-core-mvc-for-video-audio-streaming)」
```csharp
service.EnableRangeRequest();
```