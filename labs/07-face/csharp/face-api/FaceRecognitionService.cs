using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ai102.FaceApi
{
    public class FaceRecognitionService
    {
        private readonly IFaceClient faceClient;
        private const string PersonGroupId = "myfriends";

        public FaceRecognitionService(IFaceClient client)
        {
            faceClient = client;
        }

        public async Task EnsurePersonGroupExists()
        {
            try
            {
                await faceClient.PersonGroup.GetAsync(PersonGroupId);
                Console.WriteLine("Person group already exists.");
            }
            catch (APIErrorException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await faceClient.PersonGroup.CreateAsync(PersonGroupId, "My Friends");
                Console.WriteLine("Person group created.");
            }
        }

        public async Task AddPersonsToGroup()
        {
            var friend1 = await faceClient.PersonGroupPerson.CreateAsync(PersonGroupId, "Friend1");
            foreach (var imagePath in Directory.GetFiles("images/Aisha"))
            {
                using (var imageStream = File.OpenRead(imagePath))
                {
                    await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(PersonGroupId, friend1.PersonId, imageStream, detectionModel: DetectionModel.Detection03);
                }
            }

            // Repeat for other friends
        }

        public async Task TrainPersonGroup()
        {
            await faceClient.PersonGroup.TrainAsync(PersonGroupId);
            Console.WriteLine("Training initiated.");

            TrainingStatus trainingStatus;
            do
            {
                trainingStatus = await faceClient.PersonGroup.GetTrainingStatusAsync(PersonGroupId);
                Console.WriteLine($"Training status: {trainingStatus.Status}");
                await Task.Delay(1000);
            } while (trainingStatus.Status == TrainingStatusType.Running);

            Console.WriteLine("Training completed.");
        }

        public async Task RecognizeFaces(string imageFilePath)
        {
            IList<DetectedFace> detectedFaces;
            using (var imageStream = File.OpenRead(imageFilePath))
            {
                detectedFaces = await faceClient.Face.DetectWithStreamAsync(imageStream, detectionModel: DetectionModel.Detection03);
            }

            var faceIds = detectedFaces.Select(face => face.FaceId).Where(faceId => faceId.HasValue).Select(faceId => faceId.Value).ToList();
            var identifyResults = await faceClient.Face.IdentifyAsync(faceIds, PersonGroupId);

            foreach (var result in identifyResults)
            {
                Console.WriteLine($"Result of face: {result.FaceId}");
                if (result.Candidates.Count == 0)
                {
                    Console.WriteLine("No one identified");
                }
                else
                {
                    var candidateId = result.Candidates[0].PersonId;
                    var person = await faceClient.PersonGroupPerson.GetAsync(PersonGroupId, candidateId);
                    Console.WriteLine($"Identified as {person.Name}");
                }
            }
        }
    }
}
