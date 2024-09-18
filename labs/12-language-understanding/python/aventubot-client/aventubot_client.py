from dotenv import load_dotenv
import os
from datetime import datetime, date, timedelta
from azure.core.credentials import AzureKeyCredential
from azure.ai.language.conversations import ConversationAnalysisClient

def main():
    try:
        # Get Configuration Settings
        load_dotenv()
        ls_prediction_endpoint = os.getenv('LS_CONVERSATIONS_ENDPOINT')
        ls_prediction_key = os.getenv('LS_CONVERSATIONS_KEY')

        # Create a client for the Language service model
        client = ConversationAnalysisClient(ls_prediction_endpoint, AzureKeyCredential(ls_prediction_key))

        # Main loop to handle user input
        user_text = ''
        while user_text.lower() != 'quit':
            user_text = input('\nEnter some text ("quit" to stop)\n')
            if user_text.lower() != 'quit':
                # Resolve the intent and handle the intent
                conversation_prediction = resolve_intent(client, user_text)
                handle_intent(conversation_prediction, user_text)

    except Exception as ex:
        print(ex)

def resolve_intent(client: ConversationAnalysisClient, user_text):
    project_name = "AventuBot"
    deployment_name = "aventubot"
    data = {
        "kind": "Conversation",
        "analysisInput": {
            "conversationItem": {
                "participantId": "1",
                "id": "1",
                "modality": "text",
                "language": "es",
                "text": user_text
            },
            "isLoggingEnabled": False
        },
        "parameters": {
            "projectName": project_name,
            "deploymentName": deployment_name,
            "verbose": True
        }
    }

    # Send the request to the Language service model
    response = client.analyze_conversation(task=data)
    # Access the prediction part of the response
    prediction = response["result"]["prediction"]
    return prediction

def handle_intent(conversation_prediction, user_text):
    top_intent = ""
    
    # Accede correctamente a las intenciones dentro de la predicción
    if conversation_prediction["intents"][0]["confidenceScore"] > 0.5:
        top_intent = conversation_prediction["topIntent"]
    else:
        top_intent = "None"

    if top_intent == "FindActivities":
        find_activities(conversation_prediction)
    elif top_intent == "GetActivityDetails":
        get_activity_details(conversation_prediction)
    elif top_intent == "GetActivityLocation":
        get_activity_location(conversation_prediction)
    elif top_intent == "BookActivity":
        book_activity(conversation_prediction)
    else:
        print("Try asking me about adventure activities, activity details, or booking an experience.")


def find_activities(conversation_prediction):
    location = "desconocida"
    activity = "aventura"
    for entity in conversation_prediction["entities"]:
        if entity["category"] == "Location":
            location = entity["text"]
        elif entity["category"] == "Activity":
            activity = entity["text"]
    print(f"Searching for {activity} activities in {location}...")

def get_activity_details(conversation_prediction):
    activity = "aventura"
    location = "desconocida"
    for entity in conversation_prediction["entities"]:
        if entity["category"] == "Activity":
            activity = entity["text"]
        elif entity["category"] == "Location":
            location = entity["text"]
    print(f"Getting activity details for {activity} in {location}...")

def get_activity_location(conversation_prediction):
    activity = "aventura"
    for entity in conversation_prediction["entities"]:
        if entity["category"] == "Activity":
            activity = entity["text"]
    print(f"Searching for locations for {activity}...")

def book_activity(conversation_prediction):
    activity = "aventura"
    location = "desconocida"
    date_str = datetime.today().strftime("%m/%d/%Y")
    participants = "1"
    for entity in conversation_prediction["entities"]:
        if entity["category"] == "Activity":
            activity = entity["text"]
        elif entity["category"] == "Location":
            location = entity["text"]
        elif entity["category"] == "Date":
            date_str = entity["text"]
        elif entity["category"] == "Participants":
            participants = entity["text"]
    print(f"BOOK ACTIVITY: \n"
          f"ACTIVITY: {activity} \n"
          f"LOCATION: {location} \n"
          f"DATE: {date_str} \n"
          f"PARTICIPANTS: {participants} \n")

if __name__ == "__main__":
    main()
