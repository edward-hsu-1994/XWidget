XWidget.Linq
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.Linq.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Linq/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.Linq.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Linq/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

System.Linq查詢擴充，提供列舉分頁、範圍查詢、最大小值查詢等擴充

## 擴充方法範例
```csharp
public class TestClass{
    public int Score{get;set;}
    public TestClass[] Children{get;set;}
}
// ... something ...
var list = new TestClass[]{ /* ... arrayItem ...*/};
list.Between(x=>x.Score, 60, 100); // 取得指定屬性在範圍內的項目，支援IQueryable
list.Filter(x=>x.Score, 5); // 取得指定屬性值等於目標值的項目，支援IQueryable
list.Flatten(x=>x.Children); // 攤平子屬性
list.GroupBy("Score"); // 使用指定的屬性名稱Group，支援IQueryable
list.Match(new {
    Score = 5
}); // 取得符合指定物件屬性值的項目列表，支援IQueryable
list.OrderBy("Score"); // 使用指定屬性名稱Score排序，支援IQueryable
list.OrderBy(new(bool isDec, string name)[] {
    (isDec : true,name: "Score")
});
Enumerable.Range(0, 100).AsPaging(10, 10); // 轉換為分頁並skip
```