using binance_task.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace binance_task
{
    public class BinanceApi
    {
        private static HttpClient ApiClient = new HttpClient();
        public BinanceApi(string API_KEY)
        {
            if(ApiClient.BaseAddress==null) ApiClient.BaseAddress = new Uri("https://api.binance.com/api/");
            APIBase(API_KEY);
        }

        public  async Task<API_rateLimits> GetExchangeInfo()
        {
            StringBuilder builder = new StringBuilder(ApiClient.BaseAddress.AbsoluteUri);
            builder.Append("v3/exchangeInfo");
            HttpResponseMessage response = await ApiClient.GetAsync(builder.ToString());
            if (response.IsSuccessStatusCode)
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using var streamReader = new StreamReader(contentStream);
                using var jsonReader = new JsonTextReader(streamReader);
                JsonSerializer serializer = new();
                return serializer.Deserialize<API_rateLimits>(jsonReader);
            }
            else
            {
                Console.WriteLine("{0, -10}{1,-15}", (int)response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }
        
        public async Task<API_TickerPrice> GetTickerPrice()
        {
            StringBuilder builder = new StringBuilder(ApiClient.BaseAddress.AbsoluteUri);
            builder.Append("v3/ticker/price?symbol=BTCUSDT");
            HttpResponseMessage response = await ApiClient.GetAsync(builder.ToString());
            if (response.IsSuccessStatusCode)
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using var streamReader = new StreamReader(contentStream);
                using var jsonReader = new JsonTextReader(streamReader);
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<API_TickerPrice>(jsonReader);
            }
            else
            {
                Console.WriteLine("{0, -10}{1,-15}", (int)response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }

        static void APIBase(String API_KEY)
        {
            ApiClient.DefaultRequestHeaders.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ApiClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", API_KEY);
        }

        static long GetTimestamp()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
        public async Task<API_UserData_model> GetAccountInfo(String SECRET_KEY)
        {
            StringBuilder url = new StringBuilder(ApiClient.BaseAddress.AbsoluteUri);
            url.Append("v3/account");
            StringBuilder args = new StringBuilder();
            args.Append("&timestamp=");
            args.Append(GetTimestamp());
            var asignature = CreateSignature(args.ToString(), SECRET_KEY);
            url.Append("?");
            url.Append(args);
            url.Append("&signature=");
            url.Append(asignature);
            HttpResponseMessage response = await ApiClient.GetAsync(url.ToString());
            if (response.IsSuccessStatusCode)
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using var streamReader = new StreamReader(contentStream);
                using var jsonReader = new JsonTextReader(streamReader);
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<API_UserData_model>(jsonReader);
            }
            else
            {
                Console.WriteLine(url);
                Console.WriteLine(ApiClient.DefaultRequestHeaders);
                Console.WriteLine(SECRET_KEY);
                Console.WriteLine("{0, -10}{1,-15}", (int)response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }

        static string CreateSignature(string queryString, string SECRET_KEY)
        {

            byte[] keyBytes = Encoding.UTF8.GetBytes(SECRET_KEY);
            byte[] queryStringBytes = Encoding.UTF8.GetBytes(queryString);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);

            byte[] bytes = hmacsha256.ComputeHash(queryStringBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
