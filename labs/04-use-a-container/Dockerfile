# Use the base image of Azure Cognitive Services for Text Analytics Language Detection
FROM mcr.microsoft.com/azure-cognitive-services/textanalytics/language:latest

# Copy the .env file to the container
COPY .env .env

# Install necessary tools to process the .env file
RUN apk update && apk add dos2unix

# Convert the .env file to Unix format
RUN dos2unix .env

# Load the environment variables
RUN set -a && source .env && set +a

# Expose port 5000
EXPOSE 5000

# Command to start the container
CMD ["sh", "-c", "dotnet Microsoft.CognitiveServices.TextAnalytics.LanguageDetectionService.dll"]
