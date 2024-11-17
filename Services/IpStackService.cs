using Newtonsoft.Json;
using RestSharp;
using SofomoWeatherForecastAPI.Models;
using System.Text.RegularExpressions;

namespace SofomoWeatherForecastAPI.Services
{
    public class IpStackService
    {
        private const string IpStackApiKey = "fbfb90e2e0cdd9ae17341162cbcf9d3f";

        public static async Task<IpStackResponseModel> GetLocationFromIpAsync(string ipAddress)
        {
            var client = new RestClient("http://api.ipstack.com");
            var httpClient = new HttpClient();

            ipAddress = IsValidIpAddress(ipAddress) ? ipAddress : await httpClient.GetStringAsync("https://api.ipify.org");

            var request = new RestRequest($"{ipAddress}")
                .AddQueryParameter("access_key", IpStackApiKey);

            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful) { 
                return null;
            }

            return JsonConvert.DeserializeObject<IpStackResponseModel>(response.Content);
        }

        private static bool IsValidIpAddress(string ipAddress)
        {
            string pattern = @"^(([0-9]{1,3}\.){3}[0-9]{1,3})$|([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$";
            return Regex.IsMatch(ipAddress, pattern);
        }
    }
}
