XWidget.Reflection
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.Reflection.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Reflection/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.Reflection.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Reflection/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

常用System.Reflection擴充以及動態物件、Expression擴充

## 實用方法
### AccessExpressionUtility
```csharp
public class TestClass{
    public string Name{get;set;}
}

// ... something ...
AccessExpressionUtility.CreateAccessFunc<TestClass>("Name"); // 產生x=>x.Name的Expression
AccessExpressionUtility.CreateAccessExpressionFunc<TestClass>("Name"); // 產生x=>x.Name的Func
```

### ExpandoObjectExtension
```csharp
var obj = new ExpandoObject();
obj.TryAdd("Id", "1");

var type = obj.CreateAnonymousType(); // 透過ExpandoObject建立匿名類型
```

### ExpandoObjectUtility
```csharp
public class TestClass{
}

// ... something ...
var obj = new TestClass();
dynamic test = ExpandoObjectUtility.ConvertToExpando(obj); // 將object轉換為ExpandoObject
```

### MemberInfoExtension
```csharp
public class TestClass{
    public string Name{get;set;}
    public int GetValue(int k){
        return 1;
    }
}

// ... something ...
new TestClass().GetMember(x=>x.Name); // 取得指定Member
new TestClass().GetMember(x=>x.GetValue(0)); // 取得指定Member
```

### MethodInfoExtension
```csharp
public class TestClass{
    public string Name{get;set;}
    public int GetValue(int k){
        return 1;
    }
    public T GetValue2<T>(T k){
        return k;
    }
}

// ... something ...
var instance = new TestClass();
instance.GetMember(x=>x.GetValue(0)).AsInvoke(instance); // 取得MemberInfo並嘗試轉為MethodInfo並Invoke
typeof(TestClass).GetMethod("GetValue2")
    .InvokeGeneric(new Type[]{ int }, new object[]{ 5 }); // 執行泛型方法
```