#!/bin/sh

echo "Running start-services.sh..."

# Export environment variables from the .env file
export $(grep -v '^#' /app/.env | xargs)

# Function to start Azure Cognitive Service
start_cognitive_service() {
  echo "Starting Azure Cognitive Service..."
  Eula=accept Billing=$BILLING ApiKey=$APIKEY  dotnet Microsoft.CloudAI.Containers.LanguageFastText.dll
}

# Function to start FastAPI service
start_fastapi_service() {
  echo "Starting FastAPI service..."
  uvicorn server:app --host 0.0.0.0 --port 5001 --reload
}

# Start both services in background
start_cognitive_service &
start_fastapi_service &

# Wait indefinitely to keep the container running
wait