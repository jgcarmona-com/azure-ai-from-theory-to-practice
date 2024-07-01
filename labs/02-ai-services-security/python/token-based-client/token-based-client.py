import logging
from dotenv import load_dotenv
import os
import http.client
import json
from urllib.parse import urlencode
from time import time

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

def main():
    global ai_endpoint
    global ai_key
    global ai_region
    global token_expiration
    global token

    try:
        # Get Configuration Settings
        load_dotenv()
        ai_endpoint = os.getenv('AI_SERVICE_ENDPOINT')
        ai_key = os.getenv('AI_SERVICE_KEY')
        ai_region = os.getenv('AI_SERVICE_REGION')

        # Get the initial token
        token, token_expiration = get_token(ai_key, ai_region)

        # Get user input (until they enter "quit")
        userText = ''
        while userText.lower() != 'quit':
            userText = input('Enter some text ("quit" to stop)\n')
            if userText.lower() != 'quit':
                translate_text(userText)

    except Exception as ex:
        logger.error(f"Error in main: {ex}")

def get_token(subscription_key, region):
    token_endpoint = f"https://{region}.api.cognitive.microsoft.com/sts/v1.0/issueToken"
    conn = http.client.HTTPSConnection(f"{region}.api.cognitive.microsoft.com")
    headers = {
        'Ocp-Apim-Subscription-Key': subscription_key,
        'Content-type': 'application/x-www-form-urlencoded',
        'Content-length': '0'
    }
    conn.request("POST", "/sts/v1.0/issueToken", headers=headers)
    response = conn.getresponse()
    if response.status == 200:
        token = response.read().decode("utf-8")
        return token, time() + 540  # Token is valid for 9 minutes (540 seconds)
    else:
        raise Exception("Failed to obtain the token.")

def translate_text(text):
    global token
    global token_expiration

    if time() >= token_expiration:
        token, token_expiration = get_token(ai_key, ai_region)

    try:
        # Construct the JSON request body
        jsonBody = json.dumps([{"text": text}])

        # Let's take a look at the JSON we'll send to the service
        logger.info(f"Request JSON: {jsonBody}")

        # Make an HTTP request to the REST interface
        uri = ai_endpoint.rstrip('/').replace('https://', '')
        conn = http.client.HTTPSConnection(uri)

        # Add the authentication token to the request header
        headers = {
            'Content-Type': 'application/json',
            'Authorization': f'Bearer {token}'
        }

        # Use the Translator API
        conn.request("POST", "/translate?api-version=3.0&from=en&to=es", jsonBody, headers)

        # Send the request
        response = conn.getresponse()
        data = response.read().decode("UTF-8")

        # If the call was successful, get the response
        if response.status == 200:
            # Display the JSON response in full (just so we can see it)
            results = json.loads(data)
            logger.info(f"Response JSON: {json.dumps(results, indent=2)}")

            # Extract the translated text
            for translation in results:
                logger.info(f"\nTranslation: {translation['translations'][0]['text']}")

        else:
            # Something went wrong, write the whole response
            logger.error(f"Error response: {data}")

        conn.close()

    except Exception as ex:
        logger.error(f"Error in translate_text: {ex}")

if __name__ == "__main__":
    main()
