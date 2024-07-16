using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ai102.FaceApi
{
    public class FaceAnalysisService
    {
        private readonly IFaceClient faceClient;

        public FaceAnalysisService(IFaceClient client)
        {
            faceClient = client;
        }

        public async Task AnalyzeFaces(string imageFilePath)
        {
            IList<DetectedFace> detectedFaces;
            try
            {
                using (var imageStream = File.OpenRead(imageFilePath))
                {
                    detectedFaces = await faceClient.Face.DetectWithStreamAsync(imageStream, returnFaceAttributes: new List<FaceAttributeType> {
                        FaceAttributeType.Age,
                        FaceAttributeType.Gender,
                        FaceAttributeType.Emotion,
                        FaceAttributeType.Smile,
                        FaceAttributeType.Glasses,
                        FaceAttributeType.Hair
                    }, detectionModel: DetectionModel.Detection03);
                }

                foreach (var face in detectedFaces)
                {
                    Console.WriteLine($"Face detected with ID: {face.FaceId}");
                    Console.WriteLine($" - Age: {face.FaceAttributes.Age}");
                    Console.WriteLine($" - Gender: {face.FaceAttributes.Gender}");
                    var primaryEmotion = face.FaceAttributes.Emotion.ToRankedList().FirstOrDefault();
                    Console.WriteLine($" - Emotion: {primaryEmotion.Key} ({primaryEmotion.Value})");
                    Console.WriteLine($" - Smile: {face.FaceAttributes.Smile}");
                    Console.WriteLine($" - Glasses: {face.FaceAttributes.Glasses}");
                    var primaryHairColor = face.FaceAttributes.Hair.HairColor.FirstOrDefault();
                    Console.WriteLine($" - Hair: {primaryHairColor?.Color}");
                }
            }
            catch (APIErrorException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Console.WriteLine("Access denied. Please check your subscription key and permissions.");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while analyzing the faces.");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
