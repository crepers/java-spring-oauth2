using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ConsoleOauth2AppV2
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
            string httpResponse = requestOAuthService(tokenUri, "POST", "application/x-www-form-urlencoded", encodedCredentials, param);

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
            string httpResponse = requestOAuthService(userInfoUri, "GET", null, null, param);

            if(String.IsNullOrEmpty(httpResponse))
            {
                return "User info is null...";
            }

            return httpResponse;
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
        public string requestOAuthService(string uri, string httpMethod, string userAgent, string credentials, string paramstr = "")
        {
            string result = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri + paramstr);

            request.Method = httpMethod;
            request.Timeout = 30 * 1000; // 30초
//            request.Headers.Clear();
            request.Accept = "application/json";
            byte[] byteArray = Encoding.UTF8.GetBytes(paramstr);

            if (!string.IsNullOrEmpty(userAgent))
            {
                //request.Headers.Add("User-Agent", userAgent);
                
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
            }
            if (!string.IsNullOrEmpty(credentials))
            {
                request.Headers.Add("Authorization", "Basic " + credentials);
            }

            if (httpMethod.Equals("POST"))
            {
                // Get the request stream.  
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.  
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.  
                dataStream.Close();

                // Get the response.  
                WebResponse response = request.GetResponse();
                // Display the status.  
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                // Get the stream containing content returned by the server.  
                // The using block ensures the stream is automatically closed.
                using (dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.  
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.  
                    result = reader.ReadToEnd();
                }

                // Close the response.  
                response.Close();
            }
            else
            {
                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                {
                    HttpStatusCode status = resp.StatusCode;
                    Console.WriteLine(status);

                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }
            Console.WriteLine(result);

            return result;
        }
    }
}
