# Use the Azure Cognitive Services text analytics container image
FROM mcr.microsoft.com/azure-cognitive-services/textanalytics/language:latest

# Set the working directory
WORKDIR /app

# Copy the .env file into the container
COPY .env .env

# Read environment variables from the .env file and export them
ENV $(cat .env | xargs)

# Expose the port on which the container will run
EXPOSE 5000

# Command to run the Azure Cognitive Services container with the specified environment variables
CMD ["sh", "-c", "Eula=accept Billing=$BILLING_ENDPOINT ApiKey=$API_KEY /usr/local/bin/start.sh"]
