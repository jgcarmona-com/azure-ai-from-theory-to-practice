import logging
from dotenv import load_dotenv
import os
from azure.cognitiveservices.vision.customvision.prediction import CustomVisionPredictionClient
from msrest.authentication import ApiKeyCredentials

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# Load environment variables
load_dotenv()

# Configuration variables
ai_service_name = os.getenv('AI_SERVICE_NAME')
ai_service_key = os.getenv('AI_SERVICE_KEY')
model_id = os.getenv('AZURE_VISION_MODEL_ID')
iteration_name = os.getenv('AZURE_VISION_ITERATION_NAME')
confidence_threshold = float(os.getenv('AZURE_VISION_CONFIDENCE_THRESHOLD')) 


# Test images folder (relative path to main.py)
TEST_IMAGES_FOLDER = os.path.join(os.path.dirname(__file__), '../../test-images')

def main():
    try:
        # Authenticate the client
        service_endpoint = f"https://{ai_service_name}.cognitiveservices.azure.com/"
        credentials = ApiKeyCredentials(in_headers={"Prediction-key": ai_service_key})
        predictor = CustomVisionPredictionClient(service_endpoint, credentials)

        # Iterate over all images in the test folder
        for image_file in os.listdir(TEST_IMAGES_FOLDER):
            if image_file.endswith(('.jpg', '.jpeg', '.png')):
                image_path = os.path.join(TEST_IMAGES_FOLDER, image_file)

                # Read the image
                with open(image_path, "rb") as image:
                    image_data = image.read()

                # Perform image prediction
                response = predictor.classify_image(model_id, iteration_name, image_data)

                # Process and print results
                if response and response.predictions:
                    top_prediction = max(response.predictions, key=lambda p: p.probability)
                    if top_prediction.probability >= confidence_threshold:
                        logger.info(f"{image_file} --> {top_prediction.tag_name} --> {top_prediction.probability:.2f}")
                    else:
                        logger.info(f"{image_file} --> Uncertain --> {top_prediction.probability:.2f} (Best guess: {top_prediction.tag_name})")
                else:
                    logger.error("No prediction result received for image: %s", image_path)

    except Exception as ex:
        logger.error(ex)

if __name__ == "__main__":
    main()
