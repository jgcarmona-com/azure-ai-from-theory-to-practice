using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Ai102.ReadText
{
    class Program
    {
        static void Main()
        {
            try
            {
                // Get config settings from appsettings.json
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiSvcEndpoint = configuration["AIServicesEndpoint"];
                string aiSvcKey = configuration["AIServicesKey"];

                var imageProcessingService = new ImageProcessingService(aiSvcEndpoint, aiSvcKey);
                var documentService = new DocumentProcessingService(aiSvcEndpoint, aiSvcKey);

                string input = string.Empty;
                while (true)
                {
                    Console.WriteLine("Select an option:");
                    Console.WriteLine("1. Process Images");
                    Console.WriteLine("2. Extract text from PDF (Not Implemented)");

                    var option = Console.ReadLine();

                    if (input == "q" || input == "quit")
                    {
                        break;
                    }

                    switch (option)
                    {
                        case "1":
                            string testImagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "../../test-images");
                            imageProcessingService.ProcessImagesAsync(testImagesFolder).Wait();
                            break;
                        case "2":
                            string testDocumentsFolder = Path.Combine(Directory.GetCurrentDirectory(), "../../test-documents");
                            documentService.ProcessDocumentsAsync(testDocumentsFolder).Wait();                       
                            break;
                        default:
                            Console.WriteLine("Invalid option selected.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
