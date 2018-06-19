XWidget.Web.Mvc.JsonMask
=====
[![license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/XuPeiYao/XWidget)
 ![NuGet](https://img.shields.io/nuget/dt/XWidget.Web.Mvc.JsonMask.svg)
 [![Build Status](https://travis-ci.org/XuPeiYao/XWidget.svg?branch=master)](https://travis-ci.org/XuPeiYao/XWidget) [![xpy MyGet Build Status](https://www.myget.org/BuildSource/Badge/xpy?identifier=e039f3b9-1678-4c48-997b-a10eca325f39)](https://www.myget.org/)


JSON資料遮罩

## 使用情境
將EF Model作為API結果時，可能發生以下狀況。

### 場景
```csharp
public class Category{
    public Guid Id{ get;set; }

    public string Name {get;set;}

    [JsonIgnore]
    public Guid? ParentId {get;set;}
    
    public Category Parent {get;set;}

    public ICollection<Category> Children { get;set; }
}

public class CategoryController: Controller{
    //...something...

    /// <summary>
    /// 取得所有分類分頁結果
    /// </summary>
    /// <param name="keyword">關鍵字</param>
    /// <param name="skip">起始索引</param>
    /// <param name="take">取得筆數</param>
    /// <returns>分類分頁結果</returns>
    [HttpGet]
    public IEnumerable<Category> GetList(
        [FromQuery]string keyword = null,
        [FromQuery]int skip = 0,
        [FromQuery]int take = 10){
        IQueryable<Category> result = this.Context.Categories;

        // 關鍵字查詢
        if(!string.IsNullOrEmpty(keyword)){
            result = result.Where(x => EF.Functions.Like(x.Name, $"%{keyword}%"));
        }

        // AsPaging方法請見https://github.com/XuPeiYao/EzKit/blob/develop/EzKit.Linq/PagingExtension.cs
        return result.AsPaging(skip,take);
    }

    /// <summary>
    /// 取得分類樹狀結構
    /// </summary>
    /// <returns>頂層分類集合</returns>
    [HttpGet("tree")]
    public IEnumerable<Category> GetTree(){
        IEnumerable<Category> result = this.Context.Categories.ToArray();
        return result.Where(x => x.ParentId == null);
    }
}
```

### 說明
在`GetList(string,int,int)`API的回傳結果中其中的`Category`類型中又包含了`Children`的屬性。這種情況下在JSON轉換時會發生**循環參考**的問題，這種問題可以在MVC中設定

```csharp
options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
```

但如此設定下產出的資料依舊可能出現許多不必要的項目造成JSON肥大，如下:

```json
[
    {
        "Id": "1",
        "Name": "分類1",
        "Children" : [
            {
                "Id": "2",
                "Name": "分類1_1",
                "Children": [
                    {
                        "Id": "3",
                        "Name": "分類1_1_1",
                        "Children": []
                    }
                ]
            },
            {
                "Id":"4",
                "Name": "分類1_2",
                "Category": []
            }
        ]
    },
    {
        "Id": "2",
        "Name": "分類1_1",
        "Parent": {
            "Id": "1",
            "Name": "分類1",
            "Children" : [                
                {
                    "Id":"4",
                    "Name": "分類1_2",
                    "Category": []
                }
            ]
        },
        "Children": [
            {
                "Id": "3",
                "Name": "分類1_1_1",
                "Children": []
            }
        ]
    },
    {
       "Id": "3",
       "Name": "分類1_1_1",
       "Children": []
    },
    {
        "Id":"4",
        "Name": "分類1_2",
        "Category": []
    }
]
```

一般而言可以直接對`Category`中的`Children`加上`[JsonIgnore]`來避免這種情況，但是由於`GetTree()`返回的資料又必須要有這個欄位，所以不能直接使用`[JsonIgnore]`。
為了應對這種少見的情況，所以寫了這個類別庫，在`Category`的`Children`屬性加上`[JsonPropertyMask]`，如下:

```csharp
public class Category{
    public Guid Id{ get;set; }

    public string Name {get;set;}

    [JsonIgnore]
    public Guid? ParentId {get;set;}

    [JsonPropertyMask( // 當模式名稱是MyPattern時遮蔽這個屬性
        key: "MyPattern"
        Method = MaskMethods.PatternName)]
    [JsonPropertyMask( // 當控制器類型是MyController時遮蔽這個屬性
        key: typeof(MyController)
        Method = MaskMethods.Controller)]
    [JsonPropertyMask( // 當呼叫的Action返回類行為Paging<Category>時遮蔽這個屬性
        key: typeof(Paging<Category>)
        Method = MaskMethods.ReturnType)]
    [JsonPropertyMask( // 如果呼叫的Action名稱為GetList時遮蔽這個屬性
        key: nameof(CategoryController.GetList)
        Method = MaskMethods.ActionName)]
    [JsonPropertyMask( // 如果包裹此類型的類型是IEnumerable<Category>時遮蔽這個屬性
        key: tyoepf(IEnumerable<Category>)
        Method = MaskMethods.PackageType)]
    public ICollection<Category> Children { get;set; }
}
```

並且在原有`GetList(string,int,int)`方法中`return`的對象加上`ControllerExtension.Mask`方法包裝，此方法也是針對`Controller`的擴充方法，如下:
```csharp
/// <summary>
/// 取得所有分類分頁結果
/// </summary>
/// <param name="keyword">關鍵字</param>
/// <param name="skip">起始索引</param>
/// <param name="take">取得筆數</param>
/// <returns>分類分頁結果</returns>
[HttpGet]
public Paging<Category> GetList(
    [FromQuery]string keyword = null,
    [FromQuery]int skip = 0,
    [FromQuery]int take = 10){
    IQueryable<Category> result = this.Context.Categories;
    // 關鍵字查詢
    if(!string.IsNullOrEmpty(keyword)){
        result = result.Where(x => EF.Functions.Like(x.Name, $"%{keyword}%"));
    }
    // AsPaging方法請見https://github.com/XuPeiYao/EzKit/blob/develop/EzKit.Linq/PagingExtension.cs
    return this.Mask(result.AsPaging(skip,take));
}
```

除了在`return`前包裝外，也可以在`Controller`的`OnActionExecuted`針對結果進行包裝