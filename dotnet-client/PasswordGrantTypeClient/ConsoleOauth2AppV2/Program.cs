using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace ConsoleOauth2AppV2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your ID :");
            string userId = Console.ReadLine();

            Console.WriteLine("Enter your password :");
            string password = Console.ReadLine();

            OAuth2 oauth2 = new OAuth2();

            string token = oauth2.GetAccessToken(userId, password);

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Failed to get access token.");
            }
            else
            {
                oauth2.GetUserInfo(token);
            }
        }

    }
}
