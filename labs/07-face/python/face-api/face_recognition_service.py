import os
from azure.cognitiveservices.vision.face import FaceClient
from azure.cognitiveservices.vision.face.models import DetectionModel, TrainingStatusType
from msrest.authentication import CognitiveServicesCredentials
import asyncio

class FaceRecognitionService:
    def __init__(self, endpoint, key):
        self.face_client = FaceClient(endpoint, CognitiveServicesCredentials(key))
        self.person_group_id = "myfriends"

    async def ensure_person_group_exists(self):
        try:
            await self.face_client.person_group.get(person_group_id=self.person_group_id)
            print("Person group already exists.")
        except:
            await self.face_client.person_group.create(person_group_id=self.person_group_id, name="My Friends")
            print("Person group created.")

    async def add_persons_to_group(self):
        friend1 = await self.face_client.person_group_person.create(person_group_id=self.person_group_id, name="Friend1")
        for image_path in os.listdir("images/Aisha"):
            with open(os.path.join("images/Aisha", image_path), 'rb') as image_stream:
                await self.face_client.person_group_person.add_face_from_stream(self.person_group_id, friend1.person_id, image_stream, detection_model=DetectionModel.detection03)

    async def train_person_group(self):
        await self.face_client.person_group.train(self.person_group_id)
        print("Training initiated.")
        while True:
            training_status = await self.face_client.person_group.get_training_status(self.person_group_id)
            print(f"Training status: {training_status.status}")
            if training_status.status == TrainingStatusType.succeeded:
                break
            await asyncio.sleep(1)
        print("Training completed.")

    async def recognize_faces(self, image_file_path):
        with open(image_file_path, 'rb') as image_stream:
            detected_faces = await self.face_client.face.detect_with_stream(image_stream, detection_model=DetectionModel.detection03)
        face_ids = [face.face_id for face in detected_faces]
        identify_results = await self.face_client.face.identify(face_ids, self.person_group_id)
        for result in identify_results:
            print(f"Result of face: {result.face_id}")
            if not result.candidates:
                print("No one identified")
            else:
                candidate_id = result.candidates[0].person_id
                person = await self.face_client.person_group_person.get(self.person_group_id, candidate_id)
                print(f"Identified as {person.name}")
