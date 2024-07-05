#!/bin/sh

# Load environment variables from the .env file
export $(grep -v '^#' .env | xargs)

# Build the Docker image
docker build -t azure-ai-language-detection .

# Run the Docker container
docker run -p 5000:5000 --env-file .env azure-ai-language-detection
