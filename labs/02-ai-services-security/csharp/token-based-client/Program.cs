using System;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace token_based_client
{
    class Program
    {
        private static string AiSvcEndpoint;
        private static string AiSvcKey;
        private static string AiSvcRegion;
        private static string token;

        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                AiSvcEndpoint = configuration["AIServicesEndpoint"];
                AiSvcKey = configuration["AIServicesKey"];
                AiSvcRegion = configuration["AIServicesRegion"];

                // Get the initial token
                token = await GetTokenAsync(AiSvcKey, AiSvcRegion);

                // Get user input (until they enter "quit")
                string userText = "";
                while (!userText.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine("Enter some text ('quit' to stop)");
                    userText = Console.ReadLine();
                    if (!userText.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Call function to detect language
                        await TranslateText(userText);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task<string> GetTokenAsync(string subscriptionKey, string region)
        {
            string tokenEndpoint = $"https://{region}.api.cognitive.microsoft.com/sts/v1.0/issueToken";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                var response = await client.PostAsync(tokenEndpoint, null);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception("Failed to obtain the token.");
                }
            }
        }

        static async Task TranslateText(string text)
        {
            try
            {
                // Construct the JSON request body
                JArray jsonBody = new JArray(
                    new JObject(
                        new JProperty("text", text)
                    )
                );

                UTF8Encoding utf8 = new UTF8Encoding(true, true);
                byte[] encodedBytes = utf8.GetBytes(jsonBody.ToString());

                Console.WriteLine(utf8.GetString(encodedBytes, 0, encodedBytes.Length));

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var uri = $"{AiSvcEndpoint}/translate?api-version=3.0&from=en&to=es";

                HttpResponseMessage response;
                using (var content = new ByteArrayContent(encodedBytes))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await client.PostAsync(uri, content);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JArray results = JArray.Parse(responseContent);
                    Console.WriteLine(results.ToString());

                    foreach (JObject translation in results)
                    {
                        Console.WriteLine("\nTranslation: " + (string)translation["translations"][0]["text"]);
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // If unauthorized, refresh token and retry
                    token = await GetTokenAsync(AiSvcKey, AiSvcEndpoint);
                    await TranslateText(text);
                }
                else
                {
                    Console.WriteLine(response.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}