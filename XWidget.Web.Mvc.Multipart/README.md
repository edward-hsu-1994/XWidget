XWidget.Web.Mvc.Multipart
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.Web.Mvc.Multipart.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Web.Mvc.Multipart/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.Web.Mvc.Multipart.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Web.Mvc.Multipart/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

提供在MVC Controller中Multipart的進階使用，Multipart/Form格式包含JSON格式剖析

## 使用情境

### 場景
以下為資料模型
```csharp
public class Name{
    public string FirstName{get;set;}
    public string LastName{get;set;}
}

public class User{
    public Name Name{get;set;}
    public string Address{get;set;}
    public IFormFileCollection Files{get;set;}
}
```

```csharp
public class UserController: Controller{
    //...something...

    [HttpPost("a")]
    public void Post1(
        User user
        ) {
        
    }

    [HttpPost("b")]
    public void Post2(
        User user,
        IFormFileCollection files
        ) {

    }
}
```

呼叫`Post1(User)`時`User`中的`Name`屬性要受到綁定，則必須在FromData中傳
入`Name.FirstName`以及`Name.LastName`兩屬性，才可以將資料填入`User.Name`
中。

呼叫`Post2(User,IFormFileCollection)`時`User`中的屬性要受到綁定則需要前綴
`User`，將所有屬性如`Post1`那樣輸入，無法直接在FormData欄位中以`user`的方式填
入JSON格式的資料進行剖析。

如此在需要同時傳輸JSON格式欄位與傳統FromData資料時造成不變，由此撰寫這個小套件，
使用方式如下。

### 使用方式

1. 在Startup.cs中ConfigureServices方法中針對MVC進行設定，加入ModelBinderProvider
```csharp
services.AddMvc(options => {
    options.ModelBinderProviders.Insert(0, new Multipart.MultipartJsonModelBinderProvider());
});
```

2. 在所需JSON剖析的FormData欄位、參數或者類型上加入`[FromJson]`Attribute
```csharp
[FromJson]
public class Name{
    public string FirstName{get;set;}
    public string LastName{get;set;}
}
```
或
```csharp
public class User{
    [FromJson]
    public Name Name{get;set;}
    public string Address{get;set;}
    public IFormFileCollection Files{get;set;}
}
```
或
```csharp
[HttpPost]
public void Post2(
    [FromJson]User user,
    IFormFileCollection files
    ) {

}
```
照著以上步驟即可在POST Multipart/FormData格式中使用JSON剖析資料