////using System;
////using System.Collections.Generic;
////using System.IO;
////using System.Linq;
////using System.Threading;
////using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
////using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
////using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
////using Microsoft.Extensions.Configuration;

////namespace Ai102.ImageClassification
////{
////    class WorkflowExample
////    {
////        static void Main(string[] args)
////        {
////            try
////            {
////                // Load settings from appsettings.json
////                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
////                IConfigurationRoot configuration = builder.Build();

////                string trainingEndpoint = configuration["AIServices:TrainingEndpoint"];
////                string trainingKey = configuration["AIServices:TrainingKey"];
////                string predictionEndpoint = configuration["AIServices:PredictionEndpoint"];
////                string predictionKey = configuration["AIServices:PredictionKey"];
////                string predictionResourceId = configuration["AIServices:PredictionResourceId"];
////                string targetImage = configuration["AIServices:TargetImage"];
////                string publishedModelName = configuration["AIServices:PublishedModelName"];

////                // Authenticate the training client
////                CustomVisionTrainingClient trainingApi = AuthenticateTraining(trainingEndpoint, trainingKey);

////                // Authenticate the prediction client
////                CustomVisionPredictionClient predictionApi = AuthenticatePrediction(predictionEndpoint, predictionKey);

////                // Create a new project
////                Console.WriteLine("Creating new project:");
////                var project = trainingApi.CreateProject(Guid.NewGuid().ToString());

////                // Add tags to the project
////                Console.WriteLine("Adding tags...");
////                var hemlockTag = trainingApi.CreateTag(project.Id, "Hemlock");
////                var cherryTag = trainingApi.CreateTag(project.Id, "Japanese Cherry");

////                // Define the base image location
////                string baseImageLocation = Path.Combine(Directory.GetCurrentDirectory(), "../../test-images");

////                // Upload and tag images
////                Console.WriteLine("Adding images...");
////                var imageFiles = Directory.GetFiles(baseImageLocation).Select(img =>
////                {
////                    var tag = Path.GetFileName(img).Contains("1") ? hemlockTag.Id : cherryTag.Id;
////                    return new ImageFileCreateEntry(Path.GetFileName(img), File.ReadAllBytes(img)) { TagIds = new List<Guid> { tag } };
////                }).ToList();

////                var uploadResult = trainingApi.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(imageFiles));
////                if (!uploadResult.IsBatchSuccessful)
////                {
////                    Console.WriteLine("Image batch upload failed.");
////                    foreach (var image in uploadResult.Images)
////                    {
////                        Console.WriteLine("Image status: ", image.Status);
////                    }
////                    return;
////                }

////                // Train the project
////                Console.WriteLine("Training...");
////                var iteration = trainingApi.TrainProject(project.Id);
////                while (iteration.Status != "Completed")
////                {
////                    iteration = trainingApi.GetIteration(project.Id, iteration.Id);
////                    Console.WriteLine("Training status: " + iteration.Status);
////                    Console.WriteLine("Waiting 10 seconds...");
////                    Thread.Sleep(10000);
////                }

////                // Publish the iteration
////                trainingApi.PublishIteration(project.Id, iteration.Id, publishedModelName, predictionResourceId);
////                Console.WriteLine("Done!");

////                // Test the prediction endpoint
////                using (var testImage = new FileStream(Path.Combine(baseImageLocation, targetImage), FileMode.Open))
////                {
////                    var result = predictionApi.ClassifyImage(project.Id, publishedModelName, testImage);

////                    // Display the results
////                    foreach (var c in result.Predictions)
////                    {
////                        Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
////                    }
////                }
////            }
////            catch (Exception ex)
////            {
////                Console.WriteLine(ex.Message);
////            }
////        }

////        private static CustomVisionTrainingClient AuthenticateTraining(string endpoint, string trainingKey)
////        {
////            return new CustomVisionTrainingClient(new ApiKeyServiceClientCredentials(trainingKey)) { Endpoint = endpoint };
////        }

////        private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
////        {
////            return new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(predictionKey)) { Endpoint = endpoint };
////        }
////    }
////}
