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
using Azure.AI.Vision.ImageAnalysis;

namespace Ai102.ImageAnalysis
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiSvcEndpoint = configuration["AIServicesEndpoint"];
                string aiSvcKey = configuration["AIServicesKey"];
                string targetImage = configuration["TargetImage"];

                // Get image
                string workingDirectory = Directory.GetCurrentDirectory();
                //string imageFile = Path.Combine(codeDirectory, "images", targetImage);               

                // Authenticate Azure AI Vision client
                ImageAnalysisClient client = new ImageAnalysisClient(new Uri(aiSvcEndpoint), new AzureKeyCredential(aiSvcKey));

                // Analyze image
                ImageAnalyzer imageAnalyzer = new ImageAnalyzer(client, workingDirectory);
                await imageAnalyzer.AnalyzeImageAsync(targetImage);

                // Remove the background or generate a foreground matte from the image
                BackgroundRemover bgRemover = new BackgroundRemover(aiSvcEndpoint, aiSvcKey);
                await bgRemover.RemoveBackgroundAsync("images", targetImage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
