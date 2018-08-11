XWidget.Extensions
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.Extensions.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Extensions/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.Extensions.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Extensions/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

常用資料型別擴充方法

## 擴充方法列表
### ArrayExtension
```csharp
var obj = new object();
object[] objs = obj.BoxingToArray(); // 將物件包裝為陣列

var twoLevelArray = 
    new object[]{ new object[,,] {
        { { 1,2,3,4},{ 5,6,7,8} }
    };

twoLevelArray.GetLengths(); // 取得陣列各維度長度

int[] intArray = new int[];
intArray.Full(1); // 將空陣列填滿指定值
```

### BinaryFormatterExtension
```csharp
var formatter = new BinaryFormatter();

var obj = "abc123";

Assert.Equal(
    obj,
    formatter.Deserialize<string>( // 提供直接序列化、反序列化byte[]
        formatter.Serialize(obj)
    ));
```

### ByteExtension
```csharp
var stream = new byte[] { 0, 1, 2, 3, 4 }.ToStream(); // 將byte[]轉換為Stream
```

### CharExtension
```csharp
var isCJK = '永'.GetLangType() == "CJK"; // 取得語言類型
```

### EnumExtension
```csharp
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class TestAttr1 : Attribute { }
public class TestAttr2 : Attribute { }
public enum TestEnum {
    [TestAttr1]
    [TestAttr1]
    A,
    [TestAttr2]
    B,
    C,
    D
}

// ... something ...
TestEnum.A.GetEnumName(); // 取得列舉名稱
TestEnum.A.GetCustomAttributes<TestEnum, TestAttr1>(); // 取得列舉值Attribute
```

### HttpClientExtension
```csharp
HttpClient client = new HttpClient();
JToken getJson = await client.GetJsonAsync("https://jsonplaceholder.typicode.com/posts/1"); // Get取得回應並轉換為JSON結果
var result = await client.PostAsync("https://jsonplaceholder.typicode.com/posts", new StringContent("none"));
var postJson = await result.ToJsonAsync(); // 將POST回應轉換為JSON結果
```

### ObjectExtension
```csharp
public class TestClass {
    private string PrivateField = "abc";
}
public class TestClass2: TestClass {
}

// ... something ...
new TestClass().GetPrivateFieldValue<string>("PrivateField"); // 取得private欄位資料
TestClass2 child = new TestClass().ToChildType<TestClass, TestClass2>(); // 將物件轉換為子類型
```

### RandomExtension
```csharp
public enum TestEnum {
    A,
    B,
    C
}
Random rand = new Random((int)DateTime.Now.Ticks);
rand.NextDouble(min, max); // 取得範圍內的double值
rand.NextDouble(max); // 取得0至最大值範圍內的double值
rand.NextBool(); // 隨機布林值
rand.NextEnum<TestEnum>(); // 隨機列舉值
rand.NextString(new string[] { "A", "B" }) // 隨機字串值
```

### StreamExtension
```csharp
var stream = new MemoryStream();
var binary = stream.ToBytes(); // Stream轉換為byte[]
```

### StringExtension
```csharp
"0123456789".SafeSubstring(-1, 5); // "01234"，安全取得SubString
"0123456789".IsMatch(@"\d+"); // 檢驗是否符合Regex
"1,2,3,4 ,5,6, 7,8, 9".SplitByRegex(@"\W*,\W*"); // 使用Regex切割字串
"0123456789".InnerString("0", "9") // "12345678"，取得指定字串間的字串
```

### TaskExtension
```csharp
Task TestAsync(){
}
Task<int> TaskAsync2(){
    return 0;
}
// ... something ...
TestAsync().ToSync(); // 將非同步方法轉換為同步
TaskAsync2().ToSync(); // 將非同步方法轉換為同步並取得值
```

### TypeExtension
```csharp
typeof(MyClass).GetAllBaseTypes(); // 取得指令類型的所有父類型
new {}.IsAnonymousType(); // 檢驗類型是否為匿名類型
anyInstance.GetDefault(); // 取得指定實例類型的預設值，如果IsValue=false則為null
```
