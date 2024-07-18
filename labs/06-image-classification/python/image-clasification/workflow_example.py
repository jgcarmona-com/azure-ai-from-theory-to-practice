import logging
from dotenv import load_dotenv
import os
import time
from azure.cognitiveservices.vision.customvision.training import CustomVisionTrainingClient
from azure.cognitiveservices.vision.customvision.prediction import CustomVisionPredictionClient
from azure.cognitiveservices.vision.customvision.training.models import ImageFileCreateBatch, ImageFileCreateEntry
from msrest.authentication import ApiKeyCredentials

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# Load environment variables
load_dotenv()

# Configuration variables
subscription_id = os.getenv('SUBSCRIPTION_ID')
resource_group_name = os.getenv('RESOURCE_GROUP_NAME')
ai_service_name = os.getenv('AI_SERVICE_NAME')
service_key = os.getenv('AI_SERVICE_KEY')
confidence_threshold = float(os.getenv('AZURE_VISION_CONFIDENCE_THRESHOLD', 0.8))

# Construct the AI service endpoint and prediction resource ID
service_endpoint = f"https://{ai_service_name}.cognitiveservices.azure.com/"
prediction_resource_id = (
    f"/subscriptions/{subscription_id}/resourceGroups/{resource_group_name}/"
    f"providers/Microsoft.CognitiveServices/accounts/{ai_service_name}"
)

# Directories for training and test images
TRAINING_IMAGES_FOLDER = os.path.join(os.path.dirname(__file__), '../../training-images')
TEST_IMAGES_FOLDER = os.path.join(os.path.dirname(__file__), '../../test-images')

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
        project_name = "MLOps Example python"
        logger.info("Creating project...")
        project = trainer.create_project(
            project_name,
            classification_type="Multiclass"
        )

        # Add tags to the project
        tags = {}
        logger.info("Creating tags based on filenames...")
        for filename in os.listdir(TRAINING_IMAGES_FOLDER):
            if filename.endswith(('.jpg', '.jpeg', '.png')):
                tag_name = filename.split('_')[0].upper()
                if tag_name not in tags:
                    tag = trainer.create_tag(project.id, tag_name)
                    tags[tag_name] = tag.id

        # Upload and tag images
        logger.info("Adding images...")
        image_list = []
        for filename in os.listdir(TRAINING_IMAGES_FOLDER):
            if filename.endswith(('.jpg', '.jpeg', '.png')):
                tag_name = filename.split('_')[0].upper()
                tag_id = tags[tag_name]
                file_path = os.path.join(TRAINING_IMAGES_FOLDER, filename)
                with open(file_path, "rb") as image_contents:
                    image_list.append(ImageFileCreateEntry(name=filename, contents=image_contents.read(), tag_ids=[tag_id]))

        upload_result = trainer.create_images_from_files(project.id, ImageFileCreateBatch(images=image_list))
        if not upload_result.is_batch_successful:
            logger.error("Image batch upload failed.")
            for image in upload_result.images:
                logger.error("Image status: ", image.status)
            return

        # Train the project
        logger.info("Training...")
        iteration = trainer.train_project(project.id)
        while iteration.status != "Completed":
            iteration = trainer.get_iteration(project.id, iteration.id)
            logger.info("Training status: " + iteration.status)
            logger.info("Waiting 10 seconds...")
            time.sleep(10)

        # Ensure the iteration is completed
        if iteration.status == "Completed":
            # Publish the iteration
            trainer.publish_iteration(project.id, iteration.id, publish_iteration_name, prediction_resource_id)
            logger.info("Model Published!")

            logger.info("Testing the model...")
            # Test the prediction endpoint
            for image_file in os.listdir(TEST_IMAGES_FOLDER):
                if image_file.endswith(('.jpg', '.jpeg', '.png')):
                    image_path = os.path.join(TEST_IMAGES_FOLDER, image_file)

                    # Read the image
                    with open(image_path, "rb") as image:
                        image_data = image.read()

                    # Perform image prediction
                    response = predictor.classify_image(project.id, publish_iteration_name, image_data)

                    # Process and print results
                    if response and response.predictions:
                        top_prediction = max(response.predictions, key=lambda p: p.probability)
                        if top_prediction.probability >= confidence_threshold:
                            logger.info(f"{image_file} --> {top_prediction.tag_name} --> {top_prediction.probability:.2f}")
                        else:
                            logger.info(f"{image_file} --> Uncertain --> {top_prediction.probability:.2f} (Best guess: {top_prediction.tag_name})")
                    else:
                        logger.error("No prediction result received for image: %s", image_path)
        else:
            logger.error("Training did not complete successfully.")

    except Exception as ex:
        logger.error(ex)

if __name__ == "__main__":
    main()
