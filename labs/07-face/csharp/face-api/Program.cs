using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.CognitiveServices.Vision.Face;

namespace Ai102.FaceApi
{
    class Program
    {
        private static IFaceClient faceClient;

        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiServiceName = configuration["AIServiceName"];
                string cogSvcEndpoint = $"https://{aiServiceName}.cognitiveservices.azure.com/";
                string cogSvcKey = configuration["AIServiceKey"];

                // Authenticate Face client
                faceClient = new FaceClient(new ApiKeyServiceClientCredentials(cogSvcKey)) { Endpoint = cogSvcEndpoint };

                // Create service instances
                var faceDetectionService = new FaceDetectionService(faceClient);
                var faceAnalysisService = new FaceAnalysisService(faceClient);
                var faceRecognitionService = new FaceRecognitionService(faceClient);

                string command;
                do
                {
                    // Menu for face functions
                    Console.WriteLine("1: Detect faces\n2: Analyze faces\n3: Recognize faces\nAny other key to quit");
                    Console.WriteLine("Enter a number:");
                    command = Console.ReadLine();
                    switch (command)
                    {
                        case "1":
                            await faceDetectionService.DetectFaces("images/me_1.jpg");
                            break;
                        case "2":
                            await faceAnalysisService.AnalyzeFaces("images/people.jpg");
                            break;
                        case "3":
                            await faceRecognitionService.EnsurePersonGroupExists();
                            await faceRecognitionService.AddPersonsToGroup();
                            await faceRecognitionService.TrainPersonGroup();
                            await faceRecognitionService.RecognizeFaces("images/people.jpg");
                            break;
                        default:
                            Console.WriteLine("Exiting...");
                            break;
                    }
                } while (command == "1" || command == "2" || command == "3");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
