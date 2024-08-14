using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Serialization;

// Import namespaces
using Azure;
using Azure.AI.Language.Conversations;

namespace aventubot_client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string predictionEndpoint = configuration["AIServicesEndpoint"];
                string predictionKey = configuration["AIServicesKey"];

                // Create a client for the Language service model
                Uri endpoint = new Uri(predictionEndpoint);
                AzureKeyCredential credential = new AzureKeyCredential(predictionKey);

                ConversationAnalysisClient client = new ConversationAnalysisClient(endpoint, credential);

                // Main loop to handle user input
                string userText = "";
                while (userText.ToLower() != "quit")
                {
                    Console.WriteLine("\nEnter some text ('quit' to stop)");
                    userText = Console.ReadLine();
                    if (userText.ToLower() != "quit")
                    {
                        // Resolve the intent and handle the intent
                        var conversationPrediction = await ResolveIntent(client, userText);
                        HandleIntent(conversationPrediction, userText);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Method to resolve the intent using the Language service
        static async Task<dynamic> ResolveIntent(ConversationAnalysisClient client, string userText)
        {
            var projectName = "AventuBot";
            var deploymentName = "aventubot";
            var data = new
            {
                analysisInput = new
                {
                    conversationItem = new
                    {
                        text = userText,
                        id = "1",
                        participantId = "1",
                    }
                },
                parameters = new
                {
                    projectName,
                    deploymentName,
                    stringIndexType = "Utf16CodeUnit",
                },
                kind = "Conversation",
            };

            // Send the request to the Language service model
            Response response = await client.AnalyzeConversationAsync(RequestContent.Create(data));
            dynamic conversationalTaskResult = response.Content.ToDynamicFromJson(JsonPropertyNames.CamelCase);
            return conversationalTaskResult.Result.Prediction;
        }

        // Method to handle the resolved intent
        static void HandleIntent(dynamic conversationPrediction, string userText)
        {
            var topIntent = "";
            if (conversationPrediction.Intents[0].ConfidenceScore > 0.5)
            {
                topIntent = conversationPrediction.TopIntent;
            }
            else
            {
                topIntent = "None";
            }

            switch (topIntent)
            {
                case "FindActivities":
                    FindActivities(conversationPrediction);
                    break;

                case "GetActivityDetails":
                    GetActivityDetails(conversationPrediction);
                    break;

                case "GetActivityLocation":
                    GetActivityLocation(conversationPrediction);
                    break;

                case "BookActivity":
                    BookActivity(conversationPrediction);
                    break;

                default:
                    Console.WriteLine("Intenta preguntarme sobre actividades de aventura, detalles de actividades o reservar una experiencia.");
                    break;
            }
        }

        // Method to handle "FindActivities" intent
        static void FindActivities(dynamic conversationPrediction)
        {
            var location = "desconocida";
            var activity = "aventura";
            foreach (dynamic entity in conversationPrediction.Entities)
            {
                if (entity.Category == "Location")
                {
                    location = entity.Text;
                }
                else if (entity.Category == "Activity")
                {
                    activity = entity.Text;
                }
            }
            Console.WriteLine($"Searching for {activity} Activities in {location}...");
        }

        // Method to handle "GetActivityDetails" intent
        static void GetActivityDetails(dynamic conversationPrediction)
        {
            var activity = "aventura";
            var location = "desconocida";
            foreach (dynamic entity in conversationPrediction.Entities)
            {
                if (entity.Category == "Activity")
                {
                    activity = entity.Text;
                }
                else if (entity.Category == "Location")
                {
                    location = entity.Text;
                }
            }
            Console.WriteLine($"Getting Activity Details for {activity} in {location}...");
        }

        // Method to handle "GetActivityLocation" intent
        static void GetActivityLocation(dynamic conversationPrediction)
        {
            var activity = "aventura";
            foreach (dynamic entity in conversationPrediction.Entities)
            {
                if (entity.Category == "Activity")
                {
                    activity = entity.Text;
                }
            }

            Console.WriteLine($"Searching for locations for {activity}...");
        }

        // Method to handle "BookActivity" intent
        static void BookActivity(dynamic conversationPrediction)
        {
            var activity = "aventura";
            var location = "desconocida";
            var date = DateTime.Today.ToShortDateString();
            var participants = "1";
            foreach (dynamic entity in conversationPrediction.Entities)
            {
                if (entity.Category == "Activity")
                {
                    activity = entity.Text;
                }
                else if (entity.Category == "Location")
                {
                    location = entity.Text;
                }
                else if (entity.Category == "Date")
                {
                    date = entity.Text;
                }
                else if (entity.Category == "Participants")
                {
                    participants = entity.Text;
                }
            }
            Console.WriteLine(
                $"BOOK ACTIVITY: \r\n"
                + $"ACTIVITY: {activity} \r\n"
                + $"LOCATION: {location} \r\n"
                + $"DATE: {date} \r\n"
                + $"PARTICIPANTS: {participants} \r\n");
        }
    }
}
