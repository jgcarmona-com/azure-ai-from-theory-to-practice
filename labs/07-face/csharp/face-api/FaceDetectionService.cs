using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Ai102.ReadText
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Get config settings from appsettings.json
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiSvcEndpoint = configuration["AIServicesEndpoint"];
                string aiSvcKey = configuration["AIServicesKey"];

                // Authenticate Azure Computer Vision client
                ComputerVisionClient client = Authenticate(aiSvcEndpoint, aiSvcKey);

                string testImagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "../../test-images");

                // Get all image files in the directory
                string[] imageFiles = Directory.GetFiles(testImagesFolder, "*.*", SearchOption.AllDirectories)
                                               .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                           s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                           s.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                                           s.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                                                           s.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                                               .ToArray();

                if (imageFiles.Length == 0)
                {
                    Console.WriteLine($"No image files found in {testImagesFolder}.");
                    return;
                }

                // Read text from each image
                foreach (var imageFile in imageFiles)
                {
                    ReadFileLocal(client, imageFile).Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            return new ComputerVisionClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        public static async Task ReadFileLocal(ComputerVisionClient client, string imageFilePath)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine($"READ FILE FROM LOCAL: {imageFilePath}");
            Console.WriteLine("----------------------------------------------------------");

            // Read text from local file
            using (Stream imageStream = File.OpenRead(imageFilePath))
            {
                var textHeaders = await client.ReadInStreamAsync(imageStream);
                // After the request, get the operation location (operation ID)
                string operationLocation = textHeaders.OperationLocation;
                Thread.Sleep(2000);

                // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
                // We only need the ID and not the full URL
                const int numberOfCharsInOperationId = 36;
                string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

                // Extract the text
                ReadOperationResult results;
                Console.WriteLine($"Extracting text from local file {Path.GetFileName(imageFilePath)}...");
                do
                {
                    results = await client.GetReadResultAsync(Guid.Parse(operationId));
                }
                while ((results.Status == OperationStatusCodes.Running ||
                        results.Status == OperationStatusCodes.NotStarted));

                // Display the found text and draw it on the image
                Console.WriteLine();
                var textLocalFileResults = results.AnalyzeResult.ReadResults;

                using var image = Image.Load<Rgba32>(imageFilePath);
                foreach (ReadResult page in textLocalFileResults)
                {
                    foreach (Line line in page.Lines)
                    {
                        Console.WriteLine(line.Text);
                        DrawTextOnImage(image, line.Text, line.BoundingBox);
                    }
                }

                // Save the image with the suffix "_result"
                string baseName = Path.GetFileNameWithoutExtension(imageFilePath);
                string ext = Path.GetExtension(imageFilePath);
                string output_file = Path.Combine(Path.GetDirectoryName(imageFilePath), $"{baseName}_result{ext}");
                image.Save(output_file);
                Console.WriteLine($"Results saved in {output_file}");
                Console.WriteLine("----------------------------------------------------------\r\n\r\n");
            }
        }

        private static void DrawTextOnImage(Image<Rgba32> image, string text, IList<double> boundingBox)
        {
            var font = SystemFonts.CreateFont("Arial", 16);
            var options = new DrawingOptions
            {
                TextOptions = new TextOptions
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                }
            };

            var position = new PointF((float)boundingBox[0], (float)boundingBox[1]);
            var color = Color.Yellow;

            image.Mutate(ctx => ctx.DrawText(options, text, font, color, position));
        }
    }
}
