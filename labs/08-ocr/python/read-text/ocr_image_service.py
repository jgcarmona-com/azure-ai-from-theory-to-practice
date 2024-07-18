import os
import time
from azure.cognitiveservices.vision.computervision import ComputerVisionClient
from azure.cognitiveservices.vision.computervision.models import OperationStatusCodes
from msrest.authentication import CognitiveServicesCredentials
from PIL import Image, ImageDraw
import logging

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

class OcrImageService:
    def __init__(self, endpoint, key):
        self.client = ComputerVisionClient(endpoint, CognitiveServicesCredentials(key))

    def read_image(self, image_path):
        logger.info(f"Reading text from {image_path}")
        try:
            with open(image_path, 'rb') as image_stream:
                read_response = self.client.read_in_stream(image_stream, raw=True)
                read_operation_location = read_response.headers["Operation-Location"]
                operation_id = read_operation_location.split("/")[-1]

                while True:
                    read_result = self.client.get_read_result(operation_id)
                    if read_result.status not in ['notStarted', 'running']:
                        break
                    time.sleep(1)

                if read_result.status == OperationStatusCodes.succeeded:
                    image = Image.open(image_path)
                    draw = ImageDraw.Draw(image)

                    for text_result in read_result.analyze_result.read_results:
                        for line in text_result.lines:
                            self.draw_bounding_box(draw, line.bounding_box)
                            logger.info(f"Text: {line.text}")
                    
                    output_path = os.path.splitext(image_path)[0] + "_result.jpg"
                    image.save(output_path)
                    logger.info(f"Results saved in {output_path}")
                else:
                    logger.info("No text found.")
        except Exception as e:
            logger.error(f"An error occurred: {e}")

    def draw_bounding_box(self, draw, bounding_box):
        points = [(bounding_box[i], bounding_box[i + 1]) for i in range(0, len(bounding_box), 2)]
        draw.line(points + [points[0]], width=2, fill='red')
