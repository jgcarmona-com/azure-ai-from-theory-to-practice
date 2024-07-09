using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.Vision.ImageAnalysis;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Ai102.ImageAnalysis
{
    public class ImageAnalyzer
    {
        private readonly ImageAnalysisClient _client;
        private readonly string _codeDirectory;

        public ImageAnalyzer(ImageAnalysisClient client, string codeDirectory)
        {
            _client = client;
            _codeDirectory = codeDirectory;
        }

        public async Task AnalyzeImageAsync(string imageFile)
        {
            string targetImage = Path.Combine(_codeDirectory, "images", imageFile);
            Console.WriteLine($"\nAnalyzing {targetImage} \n");

            try
            {
                if (!File.Exists(targetImage))
                {
                    Console.WriteLine($"File {targetImage} does not exist.");
                    return;
                }

                using FileStream stream = new FileStream(targetImage, FileMode.Open);

                // Get result with specified features to be retrieved
                ImageAnalysisResult result = _client.Analyze(
                    BinaryData.FromStream(stream),
                    VisualFeatures.Caption |
                    VisualFeatures.DenseCaptions |
                    VisualFeatures.Objects |
                    VisualFeatures.Tags |
                    VisualFeatures.People);

                DisplayAnalysisResults(targetImage, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void DisplayAnalysisResults(string imageFile, ImageAnalysisResult result)
        {
            try
            {
                using var image = Image.Load<Rgba32>(imageFile);

                if (result.Caption != null)
                {
                    string captionText = $"{result.Caption.Text} ({result.Caption.Confidence:P})";
                    DrawTextWithBackground(image, captionText, new PointF(10, 30), 24);
                }

                if (result.Tags != null)
                {
                    string tagsText = "TAGS FOUND:\n" + string.Join("\n", result.Tags.Values.Select(tag => $"{tag.Name} ({tag.Confidence:P})"));
                    DrawTextWithBackground(image, tagsText, new PointF(image.Width - 200, 30), 12);
                }

                if (result.Objects != null)
                {
                    foreach (var obj in result.Objects.Values)
                    {
                        if (obj.Tags[0].Name.ToLower() == "person") continue;

                        var rect = new RectangleF(obj.BoundingBox.X, obj.BoundingBox.Y, obj.BoundingBox.Width, obj.BoundingBox.Height);
                        image.Mutate(ctx => ctx.Draw(Pens.Solid(Color.Black, 2), rect));
                        DrawTextWithBackground(image, obj.Tags[0].Name, new PointF(obj.BoundingBox.X, obj.BoundingBox.Y), 12);
                    }
                }

                if (result.People != null)
                {
                    foreach (var person in result.People.Values)
                    {
                        var rect = new RectangleF(person.BoundingBox.X, person.BoundingBox.Y, person.BoundingBox.Width, person.BoundingBox.Height);
                        image.Mutate(ctx => ctx.Draw(Pens.Solid(Color.Cyan, 2), rect));
                    }
                }

                if (result.DenseCaptions != null)
                {
                    string denseCaptionsText = "DENSE CAPTIONS:\n" + string.Join("\n", result.DenseCaptions.Values.Select(caption => $"{caption.Text} ({caption.Confidence:P})"));
                    DrawTextWithBackground(image, denseCaptionsText, new PointF(10, image.Height - 50), 12);
                }

                string outputFile = Path.Combine(Path.GetDirectoryName(imageFile), $"{Path.GetFileNameWithoutExtension(imageFile)}_result{Path.GetExtension(imageFile)}");
                image.Save(outputFile, new JpegEncoder());
                Console.WriteLine($"Results saved in images/{imageFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        private void DrawTextWithBackground(Image<Rgba32> image, string text, PointF position, float textSize)
        {
            string fontPath = Path.Combine(AppContext.BaseDirectory, "arial.ttf");
            var fontCollection = new FontCollection();
            var fontFamily = fontCollection.Add(fontPath);
            var font = fontFamily.CreateFont(textSize, FontStyle.Regular);


            var textOptions = new RichTextOptions(font)
            {
                Origin = position,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            var textSizeMeasure = TextMeasurer.MeasureSize(text, textOptions);
            var backgroundRect = new RectangleF(position.X - 2, position.Y - 2, textSizeMeasure.Width + 4, textSizeMeasure.Height + 4);

            image.Mutate(ctx =>
            {
                ctx.Fill(Color.White, backgroundRect);
                ctx.DrawText(textOptions, text, Brushes.Solid(Color.Black)); 
            });
        }

    }
}
