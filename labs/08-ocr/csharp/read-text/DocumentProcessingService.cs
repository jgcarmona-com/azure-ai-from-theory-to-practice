using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.Extensions.Configuration;

namespace Ai102.ReadText
{
    public class DocumentProcessingService
    {
        private readonly DocumentIntelligenceClient _client;

        public DocumentProcessingService(string endpoint, string key)
        {
            _client = new DocumentIntelligenceClient(new Uri(endpoint), new AzureKeyCredential(key));
        }

        public async Task ProcessDocumentsAsync(string folderPath)
        {
            string[] documentFiles = Directory.GetFiles(folderPath, "*.pdf", SearchOption.AllDirectories);

            if (documentFiles.Length == 0)
            {
                Console.WriteLine($"No PDF files found in {folderPath}.");
                return;
            }

            foreach (var documentFile in documentFiles)
            {
                await ReadFileAsync(documentFile);
            }
        }

        private async Task ReadFileAsync(string documentFilePath)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine($"READ FILE FROM LOCAL: {documentFilePath}");
            Console.WriteLine("----------------------------------------------------------");


            using (FileStream stream = new FileStream(documentFilePath, FileMode.Open))
            {
                var binaryData = BinaryData.FromStream(stream);
                var content = new AnalyzeDocumentContent { Base64Source = binaryData };
                var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", content);
                AnalyzeResult result = operation.Value;

                foreach (DocumentPage page in result.Pages)
                {
                    for (int i = 0; i < page.Lines.Count; i++)
                    {
                        DocumentLine line = page.Lines[i];
                        Console.WriteLine($"'{line.Content}'");
                    }
                }

                foreach (DocumentLanguage language in result.Languages)
                {
                    Console.WriteLine($"  Found language '{language.Locale}' with confidence {language.Confidence}.");
                }

                // Extract key-value pairs
                if (result.KeyValuePairs.Any())
                {
                    Console.WriteLine("Key-Value Pairs:");
                    foreach (var kvp in result.KeyValuePairs)
                    {
                        string key = kvp.Key.Content;
                        string value = kvp.Value.Content;
                        Console.WriteLine($"  {key}: {value}");
                    }
                }
                Console.WriteLine("----------------------------------------------------------\r\n\r\n");
            }

            Console.WriteLine("To learn more about Azure Document Intelligence features and capabilities yo can start with " +
            "https://learn.microsoft.com/en-us/samples/azure-samples/document-intelligence-code-samples/document-intelligence-code-samples/");
        }
    }
}
