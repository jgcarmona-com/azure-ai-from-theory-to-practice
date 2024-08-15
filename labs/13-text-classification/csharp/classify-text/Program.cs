﻿using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

// Import namespaces
using Azure;
using Azure.AI.TextAnalytics;

namespace classify_text
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
                string aiSvcEndpoint = configuration["AIServicesEndpoint"];
                string aiSvcKey = configuration["AIServicesKey"];
                string projectName = configuration["Project"];
                string deploymentName = configuration["Deployment"];

                // Create client using endpoint and key
                AzureKeyCredential credentials = new AzureKeyCredential(aiSvcKey);
                Uri endpoint = new Uri(aiSvcEndpoint);
                TextAnalyticsClient aiClient = new TextAnalyticsClient(endpoint, credentials);

                // Read each text file in the articles folder
                List<string> batchedDocuments = new List<string>();

                var folderPath = Path.GetFullPath("./articles");
                DirectoryInfo folder = new DirectoryInfo(folderPath);
                FileInfo[] files = folder.GetFiles("*.txt");
                foreach (var file in files)
                {
                    // Read the file contents
                    StreamReader sr = file.OpenText();
                    var text = sr.ReadToEnd();
                    sr.Close();
                    batchedDocuments.Add(text);
                }

                // Get Classifications
                ClassifyDocumentOperation operation = await aiClient.SingleLabelClassifyAsync(WaitUntil.Completed, batchedDocuments, projectName, deploymentName);

                int fileNo = 0;
                await foreach (ClassifyDocumentResultCollection documentsInPage in operation.Value)
                {

                    foreach (ClassifyDocumentResult documentResult in documentsInPage)
                    {
                        Console.WriteLine(files[fileNo].Name);
                        if (documentResult.HasError)
                        {
                            Console.WriteLine($"  Error!");
                            Console.WriteLine($"  Document error code: {documentResult.Error.ErrorCode}");
                            Console.WriteLine($"  Message: {documentResult.Error.Message}");
                            continue;
                        }

                        Console.WriteLine($"  Predicted the following class:");
                        Console.WriteLine();

                        foreach (ClassificationCategory classification in documentResult.ClassificationCategories)
                        {
                            Console.WriteLine($"  Category: {classification.Category}");
                            Console.WriteLine($"  Confidence score: {classification.ConfidenceScore}");
                            Console.WriteLine();
                        }
                        fileNo++;
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
