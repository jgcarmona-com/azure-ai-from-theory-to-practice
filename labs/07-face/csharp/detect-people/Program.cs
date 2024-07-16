using System;
using System.IO;
using System.Linq;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Configuration;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace detect_people
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiServiceName = configuration["AIServiceName"];
                string aiSvcEndpoint = $"https://{aiServiceName}.cognitiveservices.azure.com/";
                string aiSvcKey = configuration["AIServiceKey"];

                string workingDirectory = Directory.GetCurrentDirectory();
                string imageFile = Path.Combine(workingDirectory, "images", "people.jpg");
                Console.WriteLine($"\nAnalyzing {imageFile} \n");

                if (args.Length > 0)
                {
                    imageFile = args[0];
                }

                // Authenticate Azure AI Vision client
                var client = new ImageAnalysisClient(new Uri(aiSvcEndpoint), new AzureKeyCredential(aiSvcKey));

                // Analyze image
                AnalyzeImage(imageFile, client);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void AnalyzeImage(string imageFile, ImageAnalysisClient client)
        {
            try
            {
                Console.WriteLine($"\nAnalyzing {imageFile} \n");

                // Get image analysis
                using FileStream stream = new FileStream(imageFile, FileMode.Open);
                ImageAnalysisResult result = client.Analyze(
                    BinaryData.FromStream(stream),
                    VisualFeatures.People);


                if (result.People != null)
                {
                    Console.WriteLine("People in image:");

                    // Prepare to draw
                    using var image = Image.Load<Rgba32>(imageFile);

                    foreach (var person in result.People.Values)
                    {
                        if (person.Confidence > 0.5)
                        {
                            Console.WriteLine($" Person: (confidence: {person.Confidence:.2f}%)");
                            Console.WriteLine($" Bounding box: x={person.BoundingBox.X}, y={person.BoundingBox.Y}, width={person.BoundingBox.Width}, height={person.BoundingBox.Height}");

                            var rect = new RectangleF(person.BoundingBox.X, person.BoundingBox.Y, person.BoundingBox.Width, person.BoundingBox.Height);
                            image.Mutate(ctx => ctx.Draw(Pens.Solid(Color.FromRgba(255, 63, 0, 128), 2), rect));
                        }
                    }

                    // Save Image
                    string baseName = Path.GetFileNameWithoutExtension(imageFile);
                    string ext = Path.GetExtension(imageFile);
                    string output_file = $"images/{baseName}_result{ext}";
                    image.Save(output_file);
                    Console.WriteLine($"Results saved in {output_file}");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}
