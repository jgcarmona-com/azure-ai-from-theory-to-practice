import logging
from dotenv import load_dotenv
import os
import time
import uuid
from azure.cognitiveservices.vision.customvision.training import CustomVisionTrainingClient
from azure.cognitiveservices.vision.customvision.prediction import CustomVisionPredictionClient
from azure.cognitiveservices.vision.customvision.training.models import ImageFileCreateBatch, ImageFileCreateEntry
from msrest.authentication import ApiKeyCredentials

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# Load environment variables from the .env file
load_dotenv()

# Configuration variables from the .env file
service_endpoint = os.getenv('AI_SERVICE_ENDPOINT')
service_key = os.getenv('AI_SERVICE_KEY')
prediction_resource_id = os.getenv('AZURE_PREDICTION_RESOURCE_ID')

# Target image for analysis (relative path to main.py)
TARGET_IMAGE = os.path.join(os.path.dirname(__file__), '../../test-images/IMG_TEST_1.jpg')

def main():
    try:
        # Authenticate the training client
        credentials = ApiKeyCredentials(in_headers={"Training-key": service_key})
        trainer = CustomVisionTrainingClient(service_endpoint, credentials)

        # Authenticate the prediction client
        prediction_credentials = ApiKeyCredentials(in_headers={"Prediction-key": service_key})
        predictor = CustomVisionPredictionClient(service_endpoint, prediction_credentials)

        # Create a new project
        publish_iteration_name = "classifyModel"
        project_name = str(uuid.uuid4())
        logger.info("Creating project...")
        project = trainer.create_project(project_name)

        # Add tags to the project
        logger.info("Adding tags...")
        hemlock_tag = trainer.create_tag(project.id, "Hemlock")
        cherry_tag = trainer.create_tag(project.id, "Japanese Cherry")

        # Define the base image location
        base_image_location = os.path.join(os.path.dirname(__file__), "../../test-images")

        # Upload and tag images
        logger.info("Adding images...")
        image_list = []

        for image_num in range(1, 6):
            file_name = "IMG_TEST_{}.jpg".format(image_num)
            with open(os.path.join(base_image_location, file_name), "rb") as image_contents:
                tag_id = hemlock_tag.id if image_num % 2 == 0 else cherry_tag.id
                image_list.append(ImageFileCreateEntry(name=file_name, contents=image_contents.read(), tag_ids=[tag_id]))

        upload_result = trainer.create_images_from_files(project.id, ImageFileCreateBatch(images=image_list))
        if not upload_result.is_batch_successful:
            logger.error("Image batch upload failed.")
            for image in upload_result.images:
                logger.error("Image status: ", image.status)
            return

        # Train the project
        logger.info("Training...")
        iteration = trainer.train_project(project.id)
        while (iteration.status != "Completed"):
            iteration = trainer.get_iteration(project.id, iteration.id)
            logger.info("Training status: " + iteration.status)
            logger.info("Waiting 10 seconds...")
            time.sleep(10)

        # Publish the iteration
        trainer.publish_iteration(project.id, iteration.id, publish_iteration_name, prediction_resource_id)
        logger.info("Done!")

        # Test the prediction endpoint
        with open(os.path.join(base_image_location, "IMG_TEST_1.jpg"), "rb") as image_contents:
            results = predictor.classify_image(project.id, publish_iteration_name, image_contents.read())

            # Display the results.
            for prediction in results.predictions:
                logger.info(f"\t{prediction.tag_name}: {prediction.probability * 100:.2f}%")

    except Exception as ex:
        logger.error(ex)

if __name__ == "__main__":
    main()
