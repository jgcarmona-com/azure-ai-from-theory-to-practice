import logging
from dotenv import load_dotenv
import os
from azure.cognitiveservices.vision.customvision.prediction import CustomVisionPredictionClient
from msrest.authentication import ApiKeyCredentials

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# Load environment variables from the .env file
load_dotenv()

# Configuration variables from the .env file
endpoint = os.getenv('AZURE_VISION_ENDPOINT')
prediction_key = os.getenv('AZURE_VISION_KEY')
model_id = os.getenv('AZURE_VISION_MODEL_ID')

# Target image for analysis (relative path to main.py)
TARGET_IMAGE = os.path.join(os.path.dirname(__file__), '../../test-images/IMG_TEST_1.jpg')

def main():
    try:
        # Authenticate the client
        credentials = ApiKeyCredentials(in_headers={"Prediction-key": prediction_key})
        predictor = CustomVisionPredictionClient(endpoint, credentials)

        # Read the image
        with open(TARGET_IMAGE, "rb") as image:
            image_data = image.read()

        # Perform image prediction
        response = predictor.classify_image(model_id, image_data)

        # Print results
        if response:
            logger.info("Image classification completed.")
            logger.info("Raw response: %s", response)
            for prediction in response.predictions:
                logger.info(f"Label: {prediction.tag_name}, Confidence: {prediction.probability:.2f}")
        else:
            logger.error("No prediction result received.")

    except Exception as ex:
        logger.error(ex)

if __name__ == "__main__":
    main()
