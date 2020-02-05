using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleOauth2AppV4
{
    public class OAuth2
    {
        /// <summary>
        /// Password Grant Type으로 Access Token을 생성합니다.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public string GetAccessToken(string userId, string password)
        {
            Console.WriteLine("Get access token");

            string token = string.Empty;
            string clientId = "clientId";
            string clientSecret = "clientSecret";
            string tokenUri = "http://oauth.sample.com/oauth/token";
            string credentials = clientId + ":" + clientSecret;
            string encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
            string param = "?username=" + userId + "&password=" + password + "&grant_type=password&scope=ReadProfile";
            string httpResponse = requestOAuthService(tokenUri, HttpMethod.Post, "application/x-www-form-urlencoded", encodedCredentials, param);

            JObject obj = JObject.Parse(httpResponse);
            token = (string)obj["access_token"];

            return token;
        }

        /// <summary>
        /// 로그인 사용자 정보를 확인합니다.
        /// </summary>
        /// <param name="token">Access Token</param>
        /// <returns></returns>
        public string GetUserInfo(string token)
        {
            Console.WriteLine("Get user information");

            string userInfoUri = "http://oauth.sample.com/oauth/userinfo";
            string param = "?access_token=" + token;
            string httpResponse = requestOAuthService(userInfoUri, HttpMethod.Get, null, null, param);

            if(String.IsNullOrEmpty(httpResponse))
            {
                return "User info is null...";
            }

            JObject obj = JObject.Parse(httpResponse);
            string result = string.Empty;

            if (string.IsNullOrEmpty(obj["sabun"].ToString()))
                result = (string)obj["id"].ToString();
            else
                result = (string)obj["sabun"];

            return result;
        }


        /// <summary>
        /// Get SSO Service Result
        /// </summary>
        /// <param name="uri">Service URL</param>
        /// <param name="httpMethod">전송방식</param>
        /// <param name="userAgent">헤더값</param>
        /// <param name="credentials">헤더에 넣을 인증값</param>
        /// <param name="paramstr">추가 URL</param>
        /// <returns></returns>
        public string requestOAuthService(string uri, HttpMethod httpMethod, string userAgent, string credentials, string paramstr = "")
        {
            string result = string.Empty;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(uri + paramstr);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            if (!string.IsNullOrEmpty(userAgent))
            {
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            }
            if (!string.IsNullOrEmpty(credentials))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
            }
            
            if (httpMethod == HttpMethod.Post)
            {
                HttpResponseMessage response = client.PostAsync("", null).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            else
            {
                HttpResponseMessage response = client.GetAsync("").Result;
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }

            Console.WriteLine(result);

            return result;
        }
    }
}
