import logging
from dotenv import load_dotenv
import os
from PIL import Image, ImageDraw
import sys
from azure.core.exceptions import HttpResponseError
from matplotlib import pyplot as plt
import azure.ai.vision as sdk
from azure.core.credentials import AzureKeyCredential
from azure.ai.vision.imageanalysis import ImageAnalysisClient
from azure.ai.vision.imageanalysis.models import VisualFeatures

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# Define the base directory
BASE_DIR = os.path.dirname(os.path.abspath(__file__))

def main():
    global cv_client

    try:
        # Get Configuration Settings
        load_dotenv()
        ai_service_name = os.getenv('AI_SERVICE_NAME')
        ai_key = os.getenv('AI_SERVICE_KEY')
        service_endpoint = f"https://{ai_service_name}.cognitiveservices.azure.com/"

        # Get image
        image_filename = os.path.join(BASE_DIR, 'images', 'people.jpg')
        if len(sys.argv) > 1:
            image_filename = sys.argv[1]

        # Authenticate Azure AI Vision client
        cv_client = ImageAnalysisClient(endpoint=service_endpoint, credential=AzureKeyCredential(ai_key))
        
        with open(image_filename, "rb") as image_data:
            result = cv_client.analyze(image_data, visual_features=[VisualFeatures.PEOPLE])


        # Get people in image
        if result.people is not None:
            logger.info("People in image:")
            # Prepare to draw
            image = Image.open(image_filename)
            fig = plt.figure(figsize=(image.width / 100, image.height / 100))
            plt.axis('off')
            draw = ImageDraw.Draw(image)

            for person in [p for p in result.people.list if p.confidence > 0.5]:
                logger.info(f" Person: (confidence: {person.confidence:.2f}%)")
                logger.info(f" Bounding box: x={person.bounding_box.x}, y={person.bounding_box.y}, width={person.bounding_box.width}, height={person.bounding_box.height}")
                bounding_box = ((person.bounding_box.x, person.bounding_box.y), 
                                (person.bounding_box.x + person.bounding_box.width, person.bounding_box.y + person.bounding_box.height))
                draw.rectangle(bounding_box, outline="#ff9900", width=2)

            # Save Image
            base, ext = os.path.splitext(image_filename)
            output_file = f"{base}_result{ext}"
            plt.imshow(image)
            plt.tight_layout(pad=0)
            fig.savefig(output_file)
            logger.info('Results saved in {}'.format(output_file))



    except HttpResponseError as e:
        logger.error(f"Status code: {e.status_code}")
        logger.error(f"Reason: {e.reason}")
        logger.error(f"Message: {e.error.message}")



if __name__ == "__main__":
    main()
