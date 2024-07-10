using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using Azure;

// Import namespaces

namespace Ai102.ImageAnalysis
{
public class BackgroundRemover
    {
        private readonly string _endpoint;
        private readonly string _key;

        public BackgroundRemover(string endpoint, string key)
        {
            _endpoint = endpoint;
            _key = key;
        }

        public async Task RemoveBackgroundAsync(string path, string imageFile)
        {
            const string apiVersion = "2023-02-01-preview";
            const string mode = "backgroundRemoval"; // Can be "foregroundMatting" or "backgroundRemoval"

            Console.WriteLine("Removing background from image...");

            string url = $"{_endpoint}computervision/imageanalysis:segment?api-version={apiVersion}&mode={mode}";

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _key);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string imageUrl = $"https://github.com/jgcarmona-com/azure-ai-from-theory-to-practice/blob/main/labs/05-analyze-images/csharp/image-analysis/images/{imageFile}?raw=true";
            var body = new { url = imageUrl };
            StringContent content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                byte[] image = await response.Content.ReadAsByteArrayAsync();
                string baseFileName = Path.GetFileNameWithoutExtension(imageFile);
                string ext = Path.GetExtension(imageFile);
                string outputFile = Path.Combine(path, $"{baseFileName}_no_bg{ext}");
                await File.WriteAllBytesAsync(outputFile, image);
                Console.WriteLine($"Results saved in {outputFile}");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Background removal failed: {response.StatusCode} - {error}");
            }
        }
    }
}