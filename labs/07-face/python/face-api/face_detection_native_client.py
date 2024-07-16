import logging
from dotenv import load_dotenv
import os
from PIL import Image, ImageDraw
from matplotlib import pyplot as plt

from azure.core.credentials import AzureKeyCredential

from azure.ai.vision.face import FaceClient
from azure.ai.vision.face.models import (
    FaceDetectionModel,
    FaceRecognitionModel,
    FaceAttributeTypeDetection03,
    FaceAttributeTypeRecognition04,
)

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# Define the base directory
BASE_DIR = os.path.dirname(os.path.abspath(__file__))


class FaceDetectionNativeClient:
    def __init__(self, endpoint, key):
        self.client = FaceClient(endpoint, AzureKeyCredential(key))

    def detect_faces(self, image_file):
        logger.info('Detecting faces using SDK in {}'.format(image_file))
        with open(image_file, 'rb') as image:
            result = self.client.detect(
                image,
                detection_model=FaceDetectionModel.DETECTION_03,
                recognition_model=FaceRecognitionModel.RECOGNITION_04,
                return_face_id=True,
                return_face_attributes=[
                    FaceAttributeTypeDetection03.HEAD_POSE,
                    FaceAttributeTypeDetection03.MASK,
                    FaceAttributeTypeRecognition04.QUALITY_FOR_RECOGNITION,
                ],
                return_face_landmarks=True,
                return_recognition_model=True,
                face_id_time_to_live=120,
            )

        if not result:
            logger.info('No faces detected.')
            return

        self._draw_faces(image_file, result)

    def _draw_faces(self, image_file, result):
        image = Image.open(image_file)
        fig = plt.figure(figsize=(image.width / 100, image.height / 100))
        plt.axis('off')
        draw = ImageDraw.Draw(image)

        for idx, face in enumerate(result):
            rect = face.face_rectangle
            bounding_box = ((rect.left, rect.top), (rect.left + rect.width, rect.top + rect.height))
            draw.rectangle(bounding_box, outline='cyan', width=2)
            logger.info(f"----- Detection result: #{idx+1} -----")
            logger.info(f"Face ID: {face.face_id}")
            logger.info(f"Attributes: {face.face_attributes.as_dict()}")

        base, ext = os.path.splitext(image_file)
        output_file = f"{base}_sdk_result{ext}"
        plt.imshow(image)
        plt.tight_layout(pad=0)
        fig.savefig(output_file)
        logger.info('Results saved in {}'.format(output_file))

