using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography;
using Xunit;
using XWidget.Utilities;
using XWidget.Web.Jwt.Test.Models;

namespace XWidget.Web.Jwt.Test {
    public class TokenTest {
        [Fact]
        public void TokenSignAndVerify() {
            var token = new TestToken() {
                Header = new DefaultJwtHeader() {
                    Algorithm = SecurityAlgorithms.HmacSha256
                },
                Payload = new CommonPayload() {
                    Actor = "TestUser",
                    Audience = "TestAudience",
                    Issuer = "TestIssuer",
                    Subject = "TestTokens",
                    IssuedAt = DateTimeUtility.FromUnixTimestamp(DateTimeUtility.ToUnixTimestamp(DateTime.Now)),
                    Expires = DateTimeUtility.FromUnixTimestamp(DateTimeUtility.ToUnixTimestamp(DateTime.Now.AddDays(1)))
                }
            };

            var testKey = new SymmetricSecurityKey("TestKey".ToHash<MD5>());

            var tokenString = token.Sign(testKey);

            var isVaild = JwtTokenConvert.Verify<CommonPayload>(
                tokenString, testKey, out var verifyToken);

            Assert.True(isVaild);

            Assert.Equal(JObject.FromObject(token), JObject.FromObject(verifyToken));
        }
    }
}
