from PIL import Image, ImageDraw
import logging
import matplotlib.tri as tri
import numpy as np
import os

from azure.cognitiveservices.vision.face import FaceClient
from azure.cognitiveservices.vision.face.models import APIErrorException
from msrest.authentication import CognitiveServicesCredentials

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

class FaceDetectionService:
    def __init__(self, endpoint, key):
        self.face_client = FaceClient(endpoint, CognitiveServicesCredentials(key))

    def detect_faces(self, image_path):
        try:
            with open(image_path, 'rb') as image_stream:
                detected_faces = self.face_client.face.detect_with_stream(
                    image_stream,
                    detection_model='detection_01',
                    return_face_landmarks=True,
                    return_face_id=False
                )

            if not detected_faces:
                logger.info("No faces detected.")
                return

            # Draw rectangles and landmarks on faces
            image = Image.open(image_path)
            draw = ImageDraw.Draw(image)
            for face in detected_faces:
                self.draw_face_rectangle(draw, face.face_rectangle)
                self.draw_landmarks(draw, face.face_landmarks)

            output_path = os.path.splitext(image_path)[0] + "_result.jpg"
            image.save(output_path)
            logger.info(f"Results saved in {output_path}")

        except APIErrorException as e:
            logger.error(f"API error: {e.message} (Code: {e.response.json().get('error', {}).get('code', 'Unknown')})")
            if 'innererror' in e.response.json().get('error', {}):
                inner_error = e.response.json()['error']['innererror']
                logger.error(f"Inner error: {inner_error.get('message')} (Code: {inner_error.get('code')})")
        
        except Exception as e:
            logger.error("An error occurred: %s", e)

    def draw_face_rectangle(self, draw, face_rectangle):
        left, top = face_rectangle.left, face_rectangle.top
        right, bottom = left + face_rectangle.width, top + face_rectangle.height
        draw.rectangle([left, top, right, bottom], outline="blue", width=2)

    def draw_landmarks(self, draw, face_landmarks):
        landmarks = face_landmarks.as_dict().values()
        points = [(landmark['x'], landmark['y']) for landmark in landmarks]
    
        # Convert points to numpy array
        points_np = np.array(points)

        # Perform Delaunay triangulation
        triangulation = tri.Triangulation(points_np[:, 0], points_np[:, 1])

        # Draw triangles
        for triangle in triangulation.triangles:
            pts = [tuple(points_np[i]) for i in triangle]
            draw.line([pts[0], pts[1], pts[2], pts[0]], fill="#4444FFDD", width=1)
        
        # Draw points
        for point in points:
             draw.point([point[0], point[1]], fill="white")
