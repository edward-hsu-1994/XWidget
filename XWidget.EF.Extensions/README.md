XWidget.EF.Extensions
=====
[![license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/XuPeiYao/XWidget)
 ![NuGet](https://img.shields.io/nuget/dt/XWidget.EF.Extensions.svg)
 [![Build Status](https://travis-ci.org/XuPeiYao/XWidget.svg?branch=master)](https://travis-ci.org/XuPeiYao/XWidget) [![xpy MyGet Build Status](https://www.myget.org/BuildSource/Badge/xpy?identifier=e039f3b9-1678-4c48-997b-a10eca325f39)](https://www.myget.org/)


針對EFCore連鎖刪除的擴充

## 說明
在EntityFrameworkCore中要刪除指定的實例時，必須要確保該實例沒有相依的項目，通常可以透過外來鍵連鎖刪除，但在特定情況下需要追蹤刪除項目、自參考的外來鍵，無法使用這個方式
就需要使用到客戶端的連鎖刪除操作。

## 快速上手
```csharp
var context = new TestContext()

// 取得刪除目標
var category = context.Categories.Single(x=>x.Name == "TargetCategory");

// 刪除實例以及相關子系
context.RemoveCascade(category);

// 刪除分類名稱為Test開頭的項目與其相關子系
context.RemoveRangeCascade(context.Categories.Where(x=>x.Name.StartWith("Test")));

// 儲存變更
context.SaveChange();
```