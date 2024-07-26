import logging
from dotenv import load_dotenv
import os

from azure.core.credentials import AzureKeyCredential
from azure.ai.textanalytics import TextAnalyticsClient

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

def main():
    try:
        # Get Configuration Settings
        load_dotenv()
        ai_endpoint = os.getenv('AI_SERVICE_ENDPOINT')
        ai_key = os.getenv('AI_SERVICE_KEY')

        # Create client using endpoint and key
        credential = AzureKeyCredential(ai_key)
        ai_client = TextAnalyticsClient(endpoint=ai_endpoint, credential=credential)
        
        # Analyze each text file in the reviews folder
        reviews_folder = os.path.join(os.path.dirname(__file__), '../../reviews')
        for file_name in os.listdir(reviews_folder):
            # Read the file contents
            logger.info('\n-------------\n' + file_name)
            text = open(os.path.join(reviews_folder, file_name), encoding='utf8').read()
            logger.info('\n' + text)

            # Get language
            detectedLanguage = ai_client.detect_language(documents=[text])[0]
            logger.info('\nLanguage: {}'.format(detectedLanguage.primary_language.name))

            # Get sentiment
            sentimentAnalysis = ai_client.analyze_sentiment(documents=[text])[0]
            logger.info("\nSentiment: {}".format(sentimentAnalysis.sentiment))

            # Get key phrases
            phrases = ai_client.extract_key_phrases(documents=[text])[0].key_phrases
            if len(phrases) > 0:
                logger.info("\nKey Phrases:")
                for phrase in phrases:
                    logger.info('\t{}'.format(phrase))

            # Get entities
            entities = ai_client.recognize_entities(documents=[text])[0].entities
            if len(entities) > 0:
                logger.info("\nEntities")
                for entity in entities:
                    logger.info('\t{} ({})'.format(entity.text, entity.category))

            # Get linked entities
            entities = ai_client.recognize_linked_entities(documents=[text])[0].entities
            if len(entities) > 0:
                logger.info("\nLinks")
                for linked_entity in entities:
                    logger.info('\t{} ({})'.format(linked_entity.name, linked_entity.url))



    except Exception as ex:
        logger.error(ex)


if __name__ == "__main__":
    main()