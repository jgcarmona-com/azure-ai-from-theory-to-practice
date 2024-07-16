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
import requests

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# Define the base directory
BASE_DIR = os.path.dirname(os.path.abspath(__file__))


class FaceDetectionRestClient:
    def __init__(self, endpoint, key):
        self.endpoint = endpoint
        self.key = key

    def detect_faces(self, image_file):
        logger.info('Detecting faces using REST API in {}'.format(image_file))
        url = f"{self.endpoint}/face/v1.1-preview.1/detect"
        params = {
            'detectionModel': 'detection_03',
            'recognitionModel': 'recognition_04',
            'returnFaceId': 'true',
            'returnFaceAttributes': 'headPose,mask,qualityForRecognition',
            'returnFaceLandmarks': 'true',
            'returnRecognitionModel': 'true',
            'faceIdTimeToLive': '120'
        }
        headers = {
            'Ocp-Apim-Subscription-Key': self.key,
            'Content-Type': 'application/octet-stream'
        }
        with open(image_file, 'rb') as image:
            response = requests.post(url, params=params, headers=headers, data=image)
        
        if response.status_code != 200:
            self._handle_error(response)
            return

        result = response.json()
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
            rect = face['faceRectangle']
            bounding_box = ((rect['left'], rect['top']), (rect['left'] + rect['width'], rect['top'] + rect['height']))
            draw.rectangle(bounding_box, outline='magenta', width=2)
            logger.info(f"----- Detection result: #{idx+1} -----")
            logger.info(f"Face ID: {face['faceId']}")
            logger.info(f"Attributes: {face.get('faceAttributes', {})}")

        base, ext = os.path.splitext(image_file)
        output_file = f"{base}_rest_result{ext}"
        plt.imshow(image)
        plt.tight_layout(pad=0)
        fig.savefig(output_file)
        logger.info('Results saved in {}'.format(output_file))
        
    def _handle_error(self, response):
        if response.status_code == 403:
            error_details = response.json().get('error', {})
            inner_error = error_details.get('innererror', {})
            if inner_error.get('code') == 'UnsupportedFeature':
                logger.error("Feature is not supported. Please apply for access at https://aka.ms/facerecognition")
            else:
                logger.error(f"Error: {response.status_code} - {response.text}")
        else:
            logger.error(f"Error: {response.status_code} - {response.text}")
