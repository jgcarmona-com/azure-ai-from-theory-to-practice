using System;
using System.IO;
using System.Linq;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
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
                string modelId = configuration["ModelId"];
                string iterationName = configuration["IterationName"];  // Load iteration name from settings
                double confidenceThreshold = Convert.ToDouble(configuration["ConfidenceThreshold"]);  // Load confidence threshold

                // Test images folder (relative path to Program.cs)
                string testImagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "../../test-images");

                // Authenticate Azure Custom Vision client
                CustomVisionPredictionClient predictionApi = AuthenticatePrediction(predictionEndpoint, predictionKey);

                // Iterate over all images in the test folder
                foreach (string imageFile in Directory.GetFiles(testImagesFolder, "*.*", SearchOption.TopDirectoryOnly)
                                                      .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                                  s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                                  s.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                {
                    // Load test image
                    using FileStream testImage = new FileStream(imageFile, FileMode.Open);

                    // Make a prediction
                    TestIteration(predictionApi, modelId, iterationName, testImage, imageFile, confidenceThreshold);
                }
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

        private static void TestIteration(CustomVisionPredictionClient predictionApi, string modelId, string iterationName, Stream testImage, string imageName, double confidenceThreshold)
        {
            // Convert modelId to Guid
            Guid projectId = Guid.Parse(modelId);

            // Make a prediction against the new project
            ImagePrediction result = predictionApi.ClassifyImage(projectId, iterationName, testImage);

            // Find the top prediction
            var topPrediction = result.Predictions.OrderByDescending(p => p.Probability).FirstOrDefault();

            // Check if there's a prediction and it meets the confidence threshold
            if (topPrediction != null)
            {
                if (topPrediction.Probability >= confidenceThreshold)
                {
                    Console.WriteLine($"{Path.GetFileName(imageName)} --> {topPrediction.TagName} --> {topPrediction.Probability:P1}");
                }
                else
                {
                    Console.WriteLine($"{Path.GetFileName(imageName)} --> Uncertain --> {topPrediction.Probability:P1} (Best guess: {topPrediction.TagName})");
                }
            }
            else
            {
                Console.WriteLine($"No prediction result received for image: {imageName}");
            }
        }
    }
}
