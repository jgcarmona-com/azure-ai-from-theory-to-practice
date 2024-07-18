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
using SixLabors.ImageSharp.Drawing;

namespace read_text
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

                string testImagesFolder = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "../../test-images");

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
                Console.WriteLine($"Extracting text from local file {System.IO.Path.GetFileName(imageFilePath)}...");
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
                        DrawBoundingBox(image, line.BoundingBox.Select(b => b.Value).ToList());
                    }
                }

                // Save the image with the suffix "_result"
                string baseName = System.IO.Path.GetFileNameWithoutExtension(imageFilePath);
                string ext = System.IO.Path.GetExtension(imageFilePath);
                string output_file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(imageFilePath), $"{baseName}_result{ext}");
                image.Save(output_file);
                Console.WriteLine($"Results saved in {output_file}");
                Console.WriteLine("----------------------------------------------------------\r\n\r\n");
            }
        }

        private static void DrawBoundingBox(Image<Rgba32> image, IList<double> boundingBox)
        {
            var pen = Pens.Solid(Color.Red, 2);
            var points = boundingBox.Select(b => (float)b).ToArray();
            var polygon = new Polygon(new LinearLineSegment(
                new PointF[] {
            new PointF(points[0], points[1]),
            new PointF(points[2], points[3]),
            new PointF(points[4], points[5]),
            new PointF(points[6], points[7]),
            new PointF(points[0], points[1]) // Close the polygon
                }));

            image.Mutate(ctx => ctx.Draw(pen, polygon));
        }

    }
}
