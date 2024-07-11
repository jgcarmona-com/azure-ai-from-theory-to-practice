using System;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Extensions.Configuration;

namespace Ai102.ImageClassification
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Load settings from appsettings.json
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();

                string predictionEndpoint = configuration["AIServicesEndpoint"];
                string predictionKey = configuration["AIServicesKey"];
                string targetImage = configuration["TargetImage"];
                string modelId = configuration["ModelId"];

                // Get image
                string workingDirectory = Directory.GetCurrentDirectory();
                string imageFile = Path.Combine(workingDirectory, "../../test-images", targetImage);

                // Authenticate Azure Custom Vision client
                CustomVisionPredictionClient predictionApi = AuthenticatePrediction(predictionEndpoint, predictionKey);

                // Load test image
                using FileStream testImage = new FileStream(imageFile, FileMode.Open);

                // Make a prediction
                TestIteration(predictionApi, modelId, testImage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
        {
            // Create a prediction endpoint, passing in the obtained prediction key
            CustomVisionPredictionClient predictionApi = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
            return predictionApi;
        }

        private static void TestIteration(CustomVisionPredictionClient predictionApi, string modelId, Stream testImage)
        {
            // Convert modelId to Guid
            Guid projectId = Guid.Parse(modelId);

            // Make a prediction against the new project
            Console.WriteLine("Making a prediction:");
            var result = predictionApi.ClassifyImage(projectId, "Iteration1", testImage);

            // Loop over each prediction and write out the results
            foreach (var c in result.Predictions)
            {
                Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
            }
        }
    }
}
