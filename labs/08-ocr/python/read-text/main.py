import os
from ocr_image_service import OcrImageService
from dotenv import load_dotenv

def main():
    load_dotenv()

    endpoint = os.getenv("AI_SERVICE_ENDPOINT")
    key = os.getenv("AI_SERVICE_KEY")

    ocr_service = OcrImageService(endpoint, key)

    test_images_folder = os.path.join(os.path.dirname(__file__), "../../test-images")

    if not os.path.exists(test_images_folder):
        print(f"The directory {test_images_folder} does not exist.")
        return

    image_files = [os.path.join(test_images_folder, f) for f in os.listdir(test_images_folder) if os.path.isfile(os.path.join(test_images_folder, f)) and f.lower().endswith(('.jpg', '.jpeg', '.png', '.bmp', '.webp'))]

    if not image_files:
        print(f"No image files found in {test_images_folder}.")
        return

    for image_file in image_files:
        ocr_service.read_image(image_file)

if __name__ == "__main__":
    main()
