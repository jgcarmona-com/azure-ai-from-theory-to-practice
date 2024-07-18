using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using Microsoft.Extensions.Configuration;
using TrainingApiKeyCredentials = Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials;
using PredictionApiKeyCredentials = Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials;


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

                string subscriptionId = configuration["SubscriptionId"];
                string resourceGroupName = configuration["ResourceGroupName"];
                string aiServiceName = configuration["AIServiceName"];
                string serviceKey = configuration["AIServiceKey"];
                string iterationName = configuration["AzureVisionIterationName"];
                double confidenceThreshold = Convert.ToDouble(configuration["AzureVisionConfidenceThreshold"]);

                // Construct the AI service endpoint and prediction resource ID
                string endpoint = $"https://{aiServiceName}.cognitiveservices.azure.com/";
                string predictionResourceId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.CognitiveServices/accounts/{aiServiceName}";

                // Directories for training and test images
                string trainingImagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "../../training-images");
                string testImagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "../../test-images");

                // Authenticate Azure Custom Vision clients
                CustomVisionTrainingClient trainingApi = AuthenticateTraining(endpoint, serviceKey);
                CustomVisionPredictionClient predictionApi = AuthenticatePrediction(endpoint, serviceKey);

                // Create a new project
                Console.WriteLine("Creating new project:");
                var project = trainingApi.CreateProject("MLOps Example C#", classificationType: "Multiclass");

                // Add tags to the project
                Console.WriteLine("Adding tags...");
                var tags = new Dictionary<string, Guid>();
                foreach (var filePath in Directory.GetFiles(trainingImagesFolder))
                {
                    var fileName = Path.GetFileName(filePath);
                    if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        var tagName = fileName.Split('_')[0].ToUpper();
                        if (!tags.ContainsKey(tagName))
                        {
                            var tag = trainingApi.CreateTag(project.Id, tagName);
                            tags[tagName] = tag.Id;
                        }
                    }
                }

                // Upload and tag images
                Console.WriteLine("Adding images...");
                var imageFiles = new List<ImageFileCreateEntry>();
                foreach (var filePath in Directory.GetFiles(trainingImagesFolder))
                {
                    var fileName = Path.GetFileName(filePath);
                    // Check if the file is an image
                    if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        var tagName = fileName.Split('_')[0];
                        var tagId = tags[tagName];
                        imageFiles.Add(new ImageFileCreateEntry(fileName, File.ReadAllBytes(filePath)) { TagIds = new List<Guid> { tagId } });
                    }
                }

                var uploadResult = trainingApi.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(imageFiles));
                if (!uploadResult.IsBatchSuccessful)
                {
                    Console.WriteLine("Image batch upload failed.");
                    foreach (var image in uploadResult.Images)
                    {
                        Console.WriteLine($"Image {image.SourceUrl} status: {image.Status}");
                    }
                }

                // Train the project
                Console.WriteLine("Training...");
                var iteration = trainingApi.TrainProject(project.Id);
                while (iteration.Status != "Completed")
                {
                    iteration = trainingApi.GetIteration(project.Id, iteration.Id);
                    Console.WriteLine("Training status: " + iteration.Status);
                    Console.WriteLine("Waiting 20 seconds...");
                    Thread.Sleep(20000);
                }

                // Publish the iteration
                trainingApi.PublishIteration(project.Id, iteration.Id, iterationName, predictionResourceId);
                Console.WriteLine("Done!");

                // Test the prediction endpoint
                foreach (var imageFile in Directory.GetFiles(testImagesFolder, "*.*", SearchOption.TopDirectoryOnly)
                                              .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                          s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                          s.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                {
                    using FileStream testImage = new FileStream(imageFile, FileMode.Open);
                    var result = predictionApi.ClassifyImage(project.Id, iterationName, testImage);

                    // Display the results
                    var topPrediction = result.Predictions.OrderByDescending(p => p.Probability).FirstOrDefault();
                    if (topPrediction != null)
                    {
                        if (topPrediction.Probability >= confidenceThreshold)
                        {
                            Console.WriteLine($"{Path.GetFileName(imageFile)} --> {topPrediction.TagName} --> {topPrediction.Probability:P1}");
                        }
                        else
                        {
                            Console.WriteLine($"{Path.GetFileName(imageFile)} --> Uncertain --> {topPrediction.Probability:P1} (Best guess: {topPrediction.TagName})");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No prediction result received for image: {imageFile}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static CustomVisionTrainingClient AuthenticateTraining(string endpoint, string trainingKey)
        {
            var credentials = new TrainingApiKeyCredentials(trainingKey);
            var client = new CustomVisionTrainingClient(credentials)
            {
                Endpoint = endpoint
            };
            return client;
        }

        private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
        {
            var credentials = new PredictionApiKeyCredentials(predictionKey);
            var client = new CustomVisionPredictionClient(credentials)
            {
                Endpoint = endpoint
            };
            return client;
        }
    }
}
