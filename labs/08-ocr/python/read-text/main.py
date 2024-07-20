import os
from document_processing_service import DocumentProcessingService
from image_processing_service import ImageProcessingService
from dotenv import load_dotenv

def main():
    load_dotenv()

    endpoint = os.getenv("AI_SERVICE_ENDPOINT")
    key = os.getenv("AI_SERVICE_KEY")

    image_processing_service = ImageProcessingService(endpoint, key)
    document_service = DocumentProcessingService(endpoint, key)

    while True:
        print("Select an option:")
        print("1. Process Images")
        print("2. Extract text from PDF")
        print("Type 'q' or 'quit' to exit.")

        option = input().strip().lower()

        if option in ['q', 'quit']:
            break

        if option == '1':
            test_images_folder = os.path.join(os.path.dirname(__file__), "../../test-images")
            image_files = [os.path.join(test_images_folder, f) for f in os.listdir(test_images_folder) 
                           if os.path.isfile(os.path.join(test_images_folder, f)) and f.lower().endswith(('.jpg', '.jpeg', '.png', '.bmp', '.webp'))]

            if not image_files:
                print(f"No image files found in {test_images_folder}.")
            else:
                for image_file in image_files:
                    image_processing_service.read_image(image_file)

        elif option == '2':
            test_documents_folder = os.path.join(os.path.dirname(__file__), "../../test-documents")
            document_files = [os.path.join(test_documents_folder, f) for f in os.listdir(test_documents_folder) 
                              if os.path.isfile(os.path.join(test_documents_folder, f)) and f.lower().endswith('.pdf')]

            if not document_files:
                print(f"No PDF files found in {test_documents_folder}.")
            else:
                for document_file in document_files:
                    document_service.read_document(document_file)

        else:
            print("Invalid option selected.")

if __name__ == "__main__":
    main()
