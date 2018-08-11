XWidget.Web.Jwt
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.Web.Jwt.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Web.Jwt/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.Web.Jwt.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.Web.Jwt/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

提供JWT產生、剖析、驗證支援

## 快速上手
```csharp
public class TestToken : IJwtToken<DefaultJwtHeader, CommonPayload> {
    public DefaultJwtHeader Header { get; set; }
    public CommonPayload Payload { get; set; }
}
// ...something...
var token = new TestToken() { // 測試用JWT結構
    Header = new DefaultJwtHeader() {
        Algorithm = SecurityAlgorithms.HmacSha256 // 設定簽名演算法為SHA256
    },
    Payload = new CommonPayload() { // 使用常用JWT內容
        Actor = "TestUser",
        Audience = "TestAudience",
        Issuer = "TestIssuer",
        Subject = "TestTokens",
        IssuedAt = DateTimeUtility.FromUnixTimestamp(DateTimeUtility.ToUnixTimestamp(DateTime.Now)), // 設定起始時間
        Expires = DateTimeUtility.FromUnixTimestamp(DateTimeUtility.ToUnixTimestamp(DateTime.Now.AddDays(1))) // 設定結束時間
    }
};

// 產生簽名鑰匙
var testKey = new SymmetricSecurityKey("TestKey".ToHash<MD5>());

// 將JWT結構簽名產生JWT字串
var tokenString = token.Sign(testKey);

// 驗證JWT字串是否合法並取得剖析後的JWT結構
var isVaild = JwtTokenConvert.Verify<CommonPayload>(
    tokenString, testKey, out var verifyToken);
```

## 提供的JWT內容類型
1.CommonPayload:
常用JWT內容

2.MvcIdentityPayload:
繼承CommonPayload並支援MVC認證的JWT內容