import logging

from azure.cognitiveservices.vision.face import FaceClient
from azure.cognitiveservices.vision.face.models import APIErrorException
from azure.cognitiveservices.vision.face.models import DetectionModel, FaceAttributeType
from msrest.authentication import CognitiveServicesCredentials

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

class FaceAnalysisService:
    def __init__(self, endpoint, key):
        self.face_client = FaceClient(endpoint, CognitiveServicesCredentials(key))

    def analyze_faces(self, image_file_path):
        try:
            with open(image_file_path, 'rb') as image_stream:
                detected_faces = self.face_client.face.detect_with_stream(
                    image_stream, return_face_attributes=[
                        # FaceAttributeType.age,
                        # FaceAttributeType.gender,
                        # FaceAttributeType.emotion,
                        # FaceAttributeType.smile,
                        # FaceAttributeType.glasses,
                        # FaceAttributeType.hair
                    ], detection_model=DetectionModel.detection_03)

            for face in detected_faces:
                logger.info(f"Face detected with ID: {face.face_id}")
                logger.info(f" - Age: {face.face_attributes.age}")
                logger.info(f" - Gender: {face.face_attributes.gender}")
                primary_emotion = max(face.face_attributes.emotion.additional_properties, key=face.face_attributes.emotion.additional_properties.get)
                logger.info(f" - Emotion: {primary_emotion} ({face.face_attributes.emotion.additional_properties[primary_emotion]})")
                logger.info(f" - Smile: {face.face_attributes.smile}")
                logger.info(f" - Glasses: {face.face_attributes.glasses}")
                primary_hair_color = max(face.face_attributes.hair.hair_color, key=lambda hc: hc.confidence).color
                logger.info(f" - Hair: {primary_hair_color}")
        except APIErrorException as e:
            logger.error(f"API error: {e.message} (Code: {e.response.json().get('error', {}).get('code', 'Unknown')})")
            if 'innererror' in e.response.json().get('error', {}):
                inner_error = e.response.json()['error']['innererror']
                logger.error(f"Inner error: {inner_error.get('message')} (Code: {inner_error.get('code')})")
        except Exception as ex:
            logger.info("An error occurred while analyzing the faces.")
            logger.info(f"Error: {ex}")
