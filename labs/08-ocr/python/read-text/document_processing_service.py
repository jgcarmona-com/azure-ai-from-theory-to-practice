import logging
import os
from azure.core.credentials import AzureKeyCredential
from azure.ai.documentintelligence import DocumentIntelligenceClient
from azure.core.exceptions import HttpResponseError

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

class DocumentProcessingService:
    def __init__(self, endpoint, key):
        self.client = DocumentIntelligenceClient(endpoint=endpoint, credential=AzureKeyCredential(key))

    def process_documents(self, folder_path):
        document_files = [os.path.join(folder_path, f) for f in os.listdir(folder_path)
                          if os.path.isfile(os.path.join(folder_path, f)) and f.lower().endswith('.pdf')]

        if not document_files:
            logger.info(f"No PDF files found in {folder_path}.")
            return

        for document_file in document_files:
            self.read_document(document_file)

    def read_document(self, document_file_path):
        logger.info("----------------------------------------------------------")
        logger.info(f"READ FILE FROM LOCAL: {document_file_path}")
        logger.info("----------------------------------------------------------")

        try:
            with open(document_file_path, "rb") as document_stream:
                poller = self.client.begin_analyze_document(
                    "prebuilt-read",
                    analyze_request=document_stream,
                    content_type="application/octet-stream"
                )
                result = poller.result()

            # Detect languages.
            logger.info("\r\n----Languages detected in the document----\r\n")
            if result.languages is not None:
                for language in result.languages:
                    logger.info(f"Language code: '{language.locale}' with confidence {language.confidence}")

            # Analyze pages.
            for page in result.pages:
                logger.info(f"----Analyzing document from page #{page.page_number}----")
                logger.info(f"Page has width: {page.width} and height: {page.height}, measured with unit: {page.unit}")

                # Analyze lines.
                if page.lines:
                    for line in page.lines:                        
                        logger.info(f"'{line.content}'")
          

            logger.info("----------------------------------------------------------\r\n\r\n")

        except HttpResponseError as error:
            if error.error is not None:
                if error.error.code == "InvalidImage":
                    logger.error(f"Received an invalid image error: {error.error}")
                if error.error.code == "InvalidRequest":
                    logger.error(f"Received an invalid request error: {error.error}")
                raise
            if "Invalid request".casefold() in error.message.casefold():
                logger.error(f"Uh-oh! Seems there was an invalid request: {error}")
            raise
        except Exception as e:
            logger.error("An error occurred: %s", e)
