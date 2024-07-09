import logging
import os
from PIL import Image, ImageDraw, ImageFont
from azure.core.exceptions import HttpResponseError
from azure.ai.vision.imageanalysis import ImageAnalysisClient
from azure.ai.vision.imageanalysis.models import VisualFeatures, ImageAnalysisResult

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# Constants to configure the drawing
DRAW_CAPTIONS = True
DRAW_TAGS = True
DRAW_OBJECTS = True
DRAW_PEOPLE = True
DRAW_DENSE_CAPTIONS = True

class ImageAnalyzer:
    def __init__(self, cv_client: ImageAnalysisClient):
        self.cv_client = cv_client

    def analyze_image(self, image_filename: str) -> None:
        logger.info('Analyzing image...')

        try:
            with open(image_filename, "rb") as f:
                image_data = f.read()

            # Get result with specified features to be retrieved
            result = self.cv_client.analyze(
                image_data=image_data,
                visual_features=[
                    VisualFeatures.CAPTION,
                    VisualFeatures.DENSE_CAPTIONS,
                    VisualFeatures.TAGS,
                    VisualFeatures.OBJECTS,
                    VisualFeatures.PEOPLE]
            )

            # Display analysis results
            self._display_analysis_results(image_filename, result)

        except HttpResponseError as e:
            logger.error(f"Status code: {e.status_code}")
            logger.error(f"Reason: {e.reason}")
            logger.error(f"Message: {e.error.message}")

    def _display_analysis_results(self, image_filename: str, result: ImageAnalysisResult) -> None:
        # Open image for drawing
        image = Image.open(image_filename)
        draw = ImageDraw.Draw(image)
        font = ImageFont.load_default()
        font_large = ImageFont.truetype(os.path.join(os.path.dirname(os.path.abspath(__file__)), "arial.ttf"), 24)

        # Draw elements based on configuration
        if DRAW_CAPTIONS:
            self._draw_caption(draw, result, font_large)
        if DRAW_OBJECTS:
            self._draw_objects(draw, result, font)
        if DRAW_PEOPLE:
            self._draw_people(draw, result, font)
        if DRAW_DENSE_CAPTIONS:
            self._draw_dense_captions(draw, result, image, font)
        if DRAW_TAGS:
            self._draw_tags(draw, result, image, font)

        # Save annotated image
        base, ext = os.path.splitext(image_filename)
        output_file = f"{base}_result{ext}"
        image.save(output_file)
        logger.info(f'  Results saved in {output_file}')

    def _draw_caption(self, draw: ImageDraw, result: ImageAnalysisResult, font_large: ImageFont) -> None:
        if result.caption is not None:
            logger.info("Caption:")
            logger.info(" Caption: '{}' (confidence: {:.2f}%)".format(result.caption.text, result.caption.confidence * 100))
            caption_text = f"{result.caption.text} ({result.caption.confidence * 100:.2f}%)"
            self._draw_text_with_background(draw, (10, 10), caption_text, font_large)

    def _draw_tags(self, draw: ImageDraw, result: ImageAnalysisResult, image: Image, font: ImageFont) -> None:
        tags_text = "TAGS FOUND:\n"
        if result.tags is not None:
            logger.info('Tags:')
            tags = [f"{tag.name} ({tag.confidence:.2f}%)" for tag in result.tags.list]
            tags_text += "\n".join(tags)
            logger.info(tags_text)
            self._draw_text_with_background(draw, (image.width - 200, 10), tags_text, font)

    def _draw_objects(self, draw: ImageDraw, result: ImageAnalysisResult, font: ImageFont) -> None:
        if result.objects is not None:
            logger.info('Objects:')
            for obj in result.objects.list:
                if obj.tags[0].name.lower() == 'person':
                    continue
                logger.info(f" Object: '{obj.tags[0].name}'")
                logger.info(f" Bounding box: x={obj.bounding_box.x}, y={obj.bounding_box.y}, width={obj.bounding_box.width}, height={obj.bounding_box.height}")
                bounding_box = ((obj.bounding_box.x, obj.bounding_box.y), 
                                (obj.bounding_box.x + obj.bounding_box.width, obj.bounding_box.y + obj.bounding_box.height))
                draw.rectangle(bounding_box, outline="red", width=3)
                self._draw_text_with_background(draw, (obj.bounding_box.x, obj.bounding_box.y), obj.tags[0].name, font)

    def _draw_people(self, draw: ImageDraw, result: ImageAnalysisResult, font: ImageFont) -> None:
        if result.people is not None:
            logger.info('People:')
            for person in result.people.list:
                logger.info(f" Person: (confidence: {person.confidence:.2f}%)")
                logger.info(f" Bounding box: x={person.bounding_box.x}, y={person.bounding_box.y}, width={person.bounding_box.width}, height={person.bounding_box.height}")
                bounding_box = ((person.bounding_box.x, person.bounding_box.y), 
                                (person.bounding_box.x + person.bounding_box.width, person.bounding_box.y + person.bounding_box.height))
                draw.rectangle(bounding_box, outline="cyan", width=3)

    def _draw_dense_captions(self, draw: ImageDraw, result: ImageAnalysisResult, image: Image, font: ImageFont) -> None:
        if result.dense_captions is not None:
            logger.info("Dense Captions:")
            dense_captions_text = "DENSE CAPTIONS:\n"
            for caption in result.dense_captions.list:
                dense_caption = f"{caption.text} ({caption.confidence * 100:.2f}%)"
                logger.info(dense_caption)
                bounding_box = ((caption.bounding_box.x, caption.bounding_box.y), 
                                (caption.bounding_box.x + caption.bounding_box.width, caption.bounding_box.y + caption.bounding_box.height))
                draw.rectangle(bounding_box, outline="green", width=3)
                self._draw_text_with_background(draw, (caption.bounding_box.x, caption.bounding_box.y), caption.text, font)
                dense_captions_text += dense_caption + "\n"
            self._draw_text_with_background(draw, (10, image.height - 50), dense_captions_text, font)

    def _draw_text_with_background(self, draw: ImageDraw, position: tuple, text: str, font: ImageFont, text_color: str = "black", background_color: str = "white") -> None:
        left, top, right, bottom = draw.textbbox(position, text, font=font)
        draw.rectangle((left-3, top-3, right+3, bottom+3), fill=background_color)
        draw.text(position, text, font=font, fill=text_color)
