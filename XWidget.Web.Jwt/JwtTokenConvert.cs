using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using JWT = System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IdentityModel.Tokens.Jwt;

namespace XWidget.Web.Jwt {
    /// <summary>
    /// JWT轉換類別
    /// </summary>
    public static class JwtTokenConvert {
        private class TempToken<TJwtPayload> : IJwtToken<DefaultJwtHeader, TJwtPayload> {
            public DefaultJwtHeader Header { get; set; }
            public TJwtPayload Payload { get; set; }
        }

        private static void SetToJwtHeader(IJwtHeader source, JWT.JwtHeader target) {
            var headerJObj = JObject.FromObject(source);

            foreach (var key in headerJObj.Properties().Select(x => x.Name)) {
                target[key] = headerJObj[key];
            }

        }

        private static void SetToJwtPayload(object source, JWT.JwtPayload target) {
            var headerJObj = JObject.FromObject(source);

            foreach (var key in headerJObj.Properties().Select(x => x.Name).OrderByDescending(x => x.Length).ThenBy(x => x)) {
                target[key] = headerJObj[key];
            }
        }

        private static JObject DictionaryToJObject(Dictionary<string, object> dictionary) {
            var result = new JObject();
            foreach (var kp in dictionary) {
                if (kp.Value == null) continue;
                result[kp.Key] = JToken.FromObject(kp.Value);
            }

            return result;
        }

        public static string Sign<TJwtHeader, TJwtPayload>(
            this IJwtToken<TJwtHeader, TJwtPayload> token,
            SecurityKey signingCredentials)
            where TJwtHeader : IJwtHeader {
            var nToken = new JWT.JwtSecurityToken(signingCredentials: new SigningCredentials(signingCredentials, token.Header.Algorithm));
            SetToJwtHeader(token.Header, nToken.Header);
            SetToJwtPayload(token.Payload, nToken.Payload);

            return "bearer " + new JWT.JwtSecurityTokenHandler().WriteToken(
                nToken
            );
        }

        public static bool Verify(
            string token,
            SecurityKey signingCredentials,
            out IJwtToken<DefaultJwtHeader, dynamic> result) {
            var tempResult = new TempToken<dynamic>();
            var returnValue = Verify<TempToken<dynamic>, DefaultJwtHeader, dynamic>(token, signingCredentials, out tempResult);

            result = tempResult;
            return returnValue;
        }

        public static bool Verify<TJwtPayload>(
            string token,
            SecurityKey signingCredentials,
            out IJwtToken<DefaultJwtHeader, TJwtPayload> result) {
            var tempResult = new TempToken<TJwtPayload>();
            var returnValue = Verify<TempToken<TJwtPayload>, DefaultJwtHeader, TJwtPayload>(token, signingCredentials, out tempResult);

            result = tempResult;
            return returnValue;
        }

        public static bool Verify<TToken, TJwtPayload>(
            string token,
            SecurityKey signingCredentials,
            out TToken result)
            where TToken : class, IJwtToken<DefaultJwtHeader, TJwtPayload> {
            return Verify<TToken, DefaultJwtHeader, TJwtPayload>(token, signingCredentials, out result);
        }

        public static bool Verify<TToken, TJwtHeader, TJwtPayload>(
            string token,
            SecurityKey signingCredentials,
            out TToken result)
            where TToken : class, IJwtToken<TJwtHeader, TJwtPayload>
            where TJwtHeader : IJwtHeader {
            return Verify<TToken, TJwtHeader, TJwtPayload>(token, new TokenValidationParameters() {
                IssuerSigningKey = signingCredentials,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateActor = false,
                ValidateLifetime = false,
                ValidateTokenReplay = false
            }, out result);
        }

        public static bool Verify<TToken, TJwtHeader, TJwtPayload>(
            string token,
            TokenValidationParameters validationParameters,
            out TToken result)
            where TToken : class, IJwtToken<TJwtHeader, TJwtPayload>
            where TJwtHeader : IJwtHeader {
            result = default(TToken);

            try {
                var nToken = new JWT.JwtSecurityToken(token.Split(' ').Last());

                var header = (TJwtHeader)DictionaryToJObject(nToken.Header).ToObject(typeof(TJwtHeader));
                var payload = (TJwtPayload)DictionaryToJObject(nToken.Payload).ToObject(typeof(TJwtPayload));

                result = (TToken)FormatterServices.GetUninitializedObject(typeof(TToken));
                result.Header = header;
                result.Payload = payload;

                if (validationParameters == null) { //不走驗證
                    return true;
                }

                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token.Split(' ').Last(), validationParameters, out _);

                return true;
            } catch {
                return false;
            }
        }
    }
}
