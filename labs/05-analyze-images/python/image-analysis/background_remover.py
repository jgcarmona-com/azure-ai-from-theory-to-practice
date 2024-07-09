import logging
import os
import requests

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

class BackgroundRemover:
    def __init__(self, endpoint: str, key: str):
        self.endpoint = endpoint
        self.key = key

    def remove_background(self, path: str, image_file: str) -> None:
        api_version = "2023-02-01-preview"
        mode = "backgroundRemoval"  # Can be "foregroundMatting" or "backgroundRemoval"
        
        logger.info('Removing background from image...')

        url = f"{self.endpoint}computervision/imageanalysis:segment?api-version={api_version}&mode={mode}"

        headers = {
            "Ocp-Apim-Subscription-Key": self.key, 
            "Content-Type": "application/json"
        }

        image_url = f"https://github.com/MicrosoftLearning/mslearn-ai-vision/blob/main/Labfiles/01-analyze-images/Python/image-analysis/images/{image_file}?raw=true"
        body = {"url": image_url}

        response = requests.post(url, headers=headers, json=body)

        if response.status_code == 200:
            image = response.content
            base, ext = os.path.splitext(image_file)
            output_file = os.path.join(path, f"{base}_no_bg{ext}")
            with open(output_file, "wb") as file:
                file.write(image)
            logger.info(f'Results saved in {output_file}\n')
        else:
            logger.error(f"Background removal failed: {response.status_code} - {response.text}")