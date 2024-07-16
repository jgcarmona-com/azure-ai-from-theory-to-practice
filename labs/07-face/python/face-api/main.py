import os
import asyncio
from dotenv import load_dotenv

from face_detection_service import FaceDetectionService
from face_analysis_service import FaceAnalysisService
from face_recognition_service import FaceRecognitionService

# Define the base directory
BASE_DIR = os.path.dirname(os.path.abspath(__file__))

def main():
    load_dotenv()
    ai_service_name = os.getenv('AI_SERVICE_NAME')
    ai_key = os.getenv('AI_SERVICE_KEY')
    service_endpoint = f"https://{ai_service_name}.cognitiveservices.azure.com/"

    face_detection_service = FaceDetectionService(service_endpoint, ai_key)
    face_analysis_service = FaceAnalysisService(service_endpoint, ai_key)
    face_recognition_service = FaceRecognitionService(service_endpoint, ai_key)

    while True:
        print("1: Detect faces\n2: Analyze faces\n3: Recognize faces\nAny other key to quit")
        command = input("Enter a number:")
        if command == '1':
            face_detection_service.detect_faces(os.path.join(BASE_DIR, 'images', 'me_2.jpg'))
        elif command == '2':
            face_analysis_service.analyze_faces(os.path.join(BASE_DIR, 'images', 'people.jpg'))
        elif command == '3':
            asyncio.run(face_recognition_service.ensure_person_group_exists())
            asyncio.run(face_recognition_service.add_persons_to_group())
            asyncio.run(face_recognition_service.train_person_group())
            asyncio.run(face_recognition_service.recognize_faces(os.path.join(BASE_DIR, 'images', 'people.jpg')))
        else:
            break

if __name__ == "__main__":
    main()
