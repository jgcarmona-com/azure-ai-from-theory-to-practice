import logging
import os
import time

from azure.cognitiveservices.vision.face import FaceClient
from azure.cognitiveservices.vision.face.models import APIErrorException, DetectionModel, TrainingStatusType
from msrest.authentication import CognitiveServicesCredentials

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

class FaceRecognitionService:
    def __init__(self, endpoint, key):
        self.face_client = FaceClient(endpoint, CognitiveServicesCredentials(key))
        self.person_group_id = "myfriends"

    def full_recognition_process(self, image_file_path):
        try:
            self._ensure_person_group_exists()
            self._add_persons_to_group()
            self._train_person_group()
            self._recognize_faces(image_file_path)
        except APIErrorException as e:
            logger.error(f"API error: {e.message} (Code: {e.response.json().get('error', {}).get('code', 'Unknown')})")
            if 'innererror' in e.response.json().get('error', {}):
                inner_error = e.response.json()['error']['innererror']
                logger.error(f"Inner error: {inner_error.get('message')} (Code: {inner_error.get('code')})")
        except Exception as e:
            logger.error("An error occurred during the full recognition process: %s", e)

    def _ensure_person_group_exists(self):
        try:
            self.face_client.person_group.get(person_group_id=self.person_group_id)
            logger.info("Person group already exists.")
        except APIErrorException as e:
            if e.response.status_code == 404:
                self.face_client.person_group.create(person_group_id=self.person_group_id, name="My Friends")
                logger.info("Person group created.")
            else:
                logger.error(f"API error: {e.message}")
                raise e

    def _add_persons_to_group(self):
        try:
            friend1 = self.face_client.person_group_person.create(person_group_id=self.person_group_id, name="Friend1")
            for image_path in os.listdir("images/Aisha"):
                with open(os.path.join("images/Aisha", image_path), 'rb') as image_stream:
                    self.face_client.person_group_person.add_face_from_stream(self.person_group_id, friend1.person_id, image_stream, detection_model=DetectionModel.detection03)
        except APIErrorException as e:
            logger.error(f"API error: {e.message}")
            raise e

    def _train_person_group(self):
        try:
            self.face_client.person_group.train(self.person_group_id)
            logger.info("Training initiated.")
            while True:
                training_status = self.face_client.person_group.get_training_status(self.person_group_id)
                logger.info(f"Training status: {training_status.status}")
                if training_status.status == TrainingStatusType.succeeded:
                    break
                time.sleep(1)
            logger.info("Training completed.")
        except APIErrorException as e:
            logger.error(f"API error: {e.message}")
            raise e

    def _recognize_faces(self, image_file_path):
        try:
            with open(image_file_path, 'rb') as image_stream:
                detected_faces = self.face_client.face.detect_with_stream(image_stream, detection_model=DetectionModel.detection03)
            if not detected_faces:
                logger.info("No faces detected.")
                return

            face_ids = [face.face_id for face in detected_faces]
            identify_results = self.face_client.face.identify(face_ids, self.person_group_id)
            for result in identify_results:
                logger.info(f"Result of face: {result.face_id}")
                if not result.candidates:
                    logger.info("No one identified")
                else:
                    candidate_id = result.candidates[0].person_id
                    person = self.face_client.person_group_person.get(self.person_group_id, candidate_id)
                    logger.info(f"Identified as {person.name}")
        except APIErrorException as e:
            logger.error(f"API error: {e.message}")
            if 'innererror' in e.response.json().get('error', {}):
                inner_error = e.response.json()['error']['innererror']
                logger.error(f"Inner error: {inner_error.get('message')} (Code: {inner_error.get('code')})")
            raise e
        except Exception as e:
            logger.error("An error occurred: %s", e)
            raise e
