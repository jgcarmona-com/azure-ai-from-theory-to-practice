using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DelaunatorSharp;
using System.Linq;

namespace Ai102.FaceApi
{
    public class FaceDetectionService
    {
        private readonly IFaceClient faceClient;

        public FaceDetectionService(IFaceClient client)
        {
            faceClient = client;
        }

        public async Task DetectFaces(string imageFilePath)
        {
            IList<DetectedFace> detectedFaces;
            using (var imageStream = File.OpenRead(imageFilePath))
            {
                detectedFaces = await faceClient.Face.DetectWithStreamAsync(imageStream, detectionModel: DetectionModel.Detection03, returnFaceLandmarks: true);
            }

            // Prepare to draw
            using var image = Image.Load<Rgba32>(imageFilePath);

            foreach (var face in detectedFaces)
            {
                DrawFaceBoundingBox(image, face.FaceRectangle);
                DrawLandmarks(image, face.FaceLandmarks);
                Console.WriteLine($"Face detected with ID: {face.FaceId}");
                Console.WriteLine($" Bounding box: x={face.FaceRectangle.Left}, y={face.FaceRectangle.Top}, width={face.FaceRectangle.Width}, height={face.FaceRectangle.Height}");
            }

            // Save Image
            string baseName = System.IO.Path.GetFileNameWithoutExtension(imageFilePath);
            string ext = System.IO.Path.GetExtension(imageFilePath);
            string output_file = $"images/{baseName}_face_detection_result{ext}";
            image.Save(output_file);
            Console.WriteLine($"Results saved in {output_file}");
        }

        private void DrawFaceBoundingBox(Image<Rgba32> image, FaceRectangle rect)
        {
            var boundingBox = new RectangleF(rect.Left, rect.Top, rect.Width, rect.Height);
            image.Mutate(ctx => ctx.Draw(Pens.Solid(Color.FromRgba(255, 0, 0, 164), 5), boundingBox));
        }

        private void DrawLandmarks(Image<Rgba32> image, FaceLandmarks landmarks)
        {
            var points = new List<PointF>
            {
                new PointF((float)landmarks.PupilLeft.X, (float)landmarks.PupilLeft.Y),
                new PointF((float)landmarks.PupilRight.X, (float)landmarks.PupilRight.Y),
                new PointF((float)landmarks.NoseTip.X, (float)landmarks.NoseTip.Y),
                new PointF((float)landmarks.MouthLeft.X, (float)landmarks.MouthLeft.Y),
                new PointF((float)landmarks.MouthRight.X, (float)landmarks.MouthRight.Y),
                new PointF((float)landmarks.EyebrowLeftOuter.X, (float)landmarks.EyebrowLeftOuter.Y),
                new PointF((float)landmarks.EyebrowLeftInner.X, (float)landmarks.EyebrowLeftInner.Y),
                new PointF((float)landmarks.EyeLeftOuter.X, (float)landmarks.EyeLeftOuter.Y),
                new PointF((float)landmarks.EyeLeftTop.X, (float)landmarks.EyeLeftTop.Y),
                new PointF((float)landmarks.EyeLeftBottom.X, (float)landmarks.EyeLeftBottom.Y),
                new PointF((float)landmarks.EyeLeftInner.X, (float)landmarks.EyeLeftInner.Y),
                new PointF((float)landmarks.EyebrowRightInner.X, (float)landmarks.EyebrowRightInner.Y),
                new PointF((float)landmarks.EyebrowRightOuter.X, (float)landmarks.EyebrowRightOuter.Y),
                new PointF((float)landmarks.EyeRightInner.X, (float)landmarks.EyeRightInner.Y),
                new PointF((float)landmarks.EyeRightTop.X, (float)landmarks.EyeRightTop.Y),
                new PointF((float)landmarks.EyeRightBottom.X, (float)landmarks.EyeRightBottom.Y),
                new PointF((float)landmarks.EyeRightOuter.X, (float)landmarks.EyeRightOuter.Y),
                new PointF((float)landmarks.NoseRootLeft.X, (float)landmarks.NoseRootLeft.Y),
                new PointF((float)landmarks.NoseRootRight.X, (float)landmarks.NoseRootRight.Y),
                new PointF((float)landmarks.NoseLeftAlarTop.X, (float)landmarks.NoseLeftAlarTop.Y),
                new PointF((float)landmarks.NoseRightAlarTop.X, (float)landmarks.NoseRightAlarTop.Y),
                new PointF((float)landmarks.NoseLeftAlarOutTip.X, (float)landmarks.NoseLeftAlarOutTip.Y),
                new PointF((float)landmarks.NoseRightAlarOutTip.X, (float)landmarks.NoseRightAlarOutTip.Y),
                new PointF((float)landmarks.UpperLipTop.X, (float)landmarks.UpperLipTop.Y),
                new PointF((float)landmarks.UpperLipBottom.X, (float)landmarks.UpperLipBottom.Y),
                new PointF((float)landmarks.UnderLipTop.X, (float)landmarks.UnderLipTop.Y),
                new PointF((float)landmarks.UnderLipBottom.X, (float)landmarks.UnderLipBottom.Y)
            };


            // Draw Delaunay triangulation lines
            DrawDelaunayTriangulation(image, points);

            foreach (var point in points)
            {
                image.Mutate(ctx => ctx.Fill(Color.White, new EllipsePolygon(point, 5)));
            }
        }

        private void DrawDelaunayTriangulation(Image<Rgba32> image, List<PointF> points)
        {
            // Convert PointF to DelaunatorSharp.Point
            var delaunayPoints = points.Select(p => new DelaunatorSharp.Point(p.X, p.Y)).Cast<IPoint>().ToArray();

            // Create Delaunator instance
            var delaunay = new Delaunator(delaunayPoints);

            // Define the fill color
            var fillColor = Color.ParseHex("#4444FFDD");

            var triangles = delaunay.Triangles;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                var p0 = delaunayPoints[triangles[i]];
                var p1 = delaunayPoints[triangles[i + 1]];
                var p2 = delaunayPoints[triangles[i + 2]];

                image.Mutate(ctx => ctx.DrawLine(fillColor, 2,
                    new PointF((float)p0.X, (float)p0.Y),
                    new PointF((float)p1.X, (float)p1.Y)));

                image.Mutate(ctx => ctx.DrawLine(fillColor, 2,
                    new PointF((float)p1.X, (float)p1.Y),
                    new PointF((float)p2.X, (float)p2.Y)));

                image.Mutate(ctx => ctx.DrawLine(fillColor, 2,
                    new PointF((float)p2.X, (float)p2.Y),
                    new PointF((float)p0.X, (float)p0.Y)));
            }
        }
    }
}