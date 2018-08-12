XWidget.Cryptography
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.Cryptography.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Cryptography/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.Cryptography.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Cryptography/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

提供加密相關實用擴充方法

## 擴充方法列表與說明
### HashHelper
本套件主要的雜湊計算類別

1.計算指定字串的雜湊值byte[]
```csharp
public static byte[] ToHash<Algorithm>(string str) where Algorithm : HashAlgorithm
```
其中泛型`Algorithm`可以為`MD5`或其他繼承自HashAlgorithm的雜湊算法

2.計算指定字串的雜湊值16進位表示字串
``` csharp
public static string ToHashString<Algorithm>(string str, bool upper = true) where Algorithm : HashAlgorithm
```
功能等同於前項方法但再返回值部分轉換為指定大小寫的16進位字串

3.計算Stream雜湊值byte[]
```csharp
public static byte[] ToHash<Algorithm>(Stream stream) where Algorithm : HashAlgorithm
```
其中泛型`Algorithm`可以為`MD5`或其他繼承自HashAlgorithm的雜湊算法

4.計算Stream雜湊值16進位表示字串
```csharp
public static string ToHashString<Algorithm>(Stream stream, bool upper = true) where Algorithm : HashAlgorithm
```
功能等同於前項方法但再返回值部分轉換為指定大小寫的16進位字串

### StringExtension
針對`string`類型的擴充方法，提供更簡單存取`HashHelper`中功能的方法
```csharp
byte[] hash_binary = "1234".ToHash<MD5>(); //取得字串1234的MD5 binary data
string hash_string = "1234".ToHashString<MD5>(); //取得字串1234的MD5的16進位表示
```

### StreamExtension
針對`Stream`類型的擴充方法，提供更簡單存取`HashHelper`中功能的方法
```csharp
var stream = new MemoryStream();
// ... something ...
byte[] hash_binary = stream.ToHash<MD5>(); //取得Stream的MD5 binary data
string hash_string = stream.ToHashString<MD5>(); //取得Stream的MD5的16進位表示
```

### FileInfoExtension
針對檔案雜湊計算擴充方法，提供更簡單存取`HashHelper`中功能的方法並計算指定檔案的雜湊值
```csharp
byte[] hash_binary = new FileInfo("<your file path>").ToHash<MD5>(); //取得指定檔案的MD5 binary data
string hash_string = new FileInfo("<your file path>").ToHashString<MD5>(); //取得指定檔案的MD5的16進位表示
```