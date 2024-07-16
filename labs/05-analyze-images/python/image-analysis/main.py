import logging
from dotenv import load_dotenv
import os
import sys
from azure.core.credentials import AzureKeyCredential
from azure.ai.vision.imageanalysis import ImageAnalysisClient
from image_analyzer import ImageAnalyzer
from background_remover import BackgroundRemover

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

TARGET_IMAGE = "me.jpg"

def main():
    try:
        # Get Configuration Settings
        load_dotenv()
        ai_endpoint = os.getenv('AI_SERVICE_ENDPOINT')
        ai_key = os.getenv('AI_SERVICE_KEY')

        # Get image path
        script_dir = os.path.dirname(os.path.abspath(__file__))
        image_file = os.path.join(script_dir, 'images', TARGET_IMAGE)
        if len(sys.argv) > 1:
            image_file = sys.argv[1]

        # Authenticate Azure AI Vision client
        cv_client = ImageAnalysisClient(endpoint=ai_endpoint, credential=AzureKeyCredential(ai_key))

        # Analyze image
        image_analyzer = ImageAnalyzer(cv_client)
        image_analyzer.analyze_image(image_file)
        
        # Background removal        
        bg_remover = BackgroundRemover(ai_endpoint, ai_key)

        script_dir = os.path.dirname(os.path.abspath(__file__))
        path = os.path.join(script_dir, 'images')
        bg_remover.remove_background(path, TARGET_IMAGE)

    except Exception as ex:
        logger.error(ex)

if __name__ == "__main__":
    main()
