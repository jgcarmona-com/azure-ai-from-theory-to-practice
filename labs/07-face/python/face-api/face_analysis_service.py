import os
from azure.cognitiveservices.vision.face import FaceClient
from azure.cognitiveservices.vision.face.models import DetectionModel, FaceAttributeType
from msrest.authentication import CognitiveServicesCredentials

class FaceAnalysisService:
    def __init__(self, endpoint, key):
        self.face_client = FaceClient(endpoint, CognitiveServicesCredentials(key))

    def analyze_faces(self, image_file_path):
        try:
            with open(image_file_path, 'rb') as image_stream:
                detected_faces = self.face_client.face.detect_with_stream(
                    image_stream, return_face_attributes=[
                        FaceAttributeType.age,
                        FaceAttributeType.gender,
                        FaceAttributeType.emotion,
                        FaceAttributeType.smile,
                        FaceAttributeType.glasses,
                        FaceAttributeType.hair
                    ], detection_model=DetectionModel.detection03)

            for face in detected_faces:
                print(f"Face detected with ID: {face.face_id}")
                print(f" - Age: {face.face_attributes.age}")
                print(f" - Gender: {face.face_attributes.gender}")
                primary_emotion = max(face.face_attributes.emotion.additional_properties, key=face.face_attributes.emotion.additional_properties.get)
                print(f" - Emotion: {primary_emotion} ({face.face_attributes.emotion.additional_properties[primary_emotion]})")
                print(f" - Smile: {face.face_attributes.smile}")
                print(f" - Glasses: {face.face_attributes.glasses}")
                primary_hair_color = max(face.face_attributes.hair.hair_color, key=lambda hc: hc.confidence).color
                print(f" - Hair: {primary_hair_color}")

        except Exception as ex:
            print("An error occurred while analyzing the faces.")
            print(f"Error: {ex}")
