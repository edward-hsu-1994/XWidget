XWidget.Utilities
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.Utilities.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Utilities/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.Utilities.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Utilities/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

常用資料處理方法

## 實用方法
### DateTimeUtility
```csharp
DateTimeUtility.ToUnixTimestamp(DateTime.Now); // 將DateTime轉換為UnixTimeStamp
DateTimeUtility.FromUnixTimestamp(timeStamp); // 將UnixTimeStamp轉換為DateTime
```

### FileUtility
```csharp
FileUtility.SecureDelete("your file path"); // 複寫檔案並刪除
```

### MathUtility
```csharp
MathUtility.GCM(4,8,12); // 最大公因數
MathUtility.LCM(4,8,12); // 最小公倍數
```

### StringUtility
```csharp
StringUtility.Spacing("aaaa測試aaaa"); // "aaaa 測試 aaaa"，在不同語系的文字內插入空白區隔
```

### TaskUtility
```csharp
await TaskUtility.LimitedTask(()=>{

}, 2000); //執行並限制Action在指定時間內執行完畢
```

### TypeUtility
```csharp
TypeUtility.GetNamespaceTypes(typeof(List<int>).Namespace); // 取得指定命名空間內的Type
TypeUtility.Parse<int>("123"); // 實作字串轉int
TypeUtility.TryParse<int>("123"); // 嘗試實作字串轉int
```