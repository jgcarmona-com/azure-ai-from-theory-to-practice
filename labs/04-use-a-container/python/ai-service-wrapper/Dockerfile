# Use the Azure Cognitive Services text analytics container image
FROM mcr.microsoft.com/azure-cognitive-services/textanalytics/language:latest

USER root

# Set the working directory
WORKDIR /app

# Copy the .env file into the container
COPY .env .env

# Read environment variables from the .env file and export them
ENV $(cat .env | xargs)

# Install Python
RUN yum install -y python3

RUN python3 --version

# Install pip packages
COPY requirements.txt .
RUN python3 -m ensurepip --upgrade && \
    python3 -m pip install --no-cache-dir -r requirements.txt

# Copy the application code
COPY . .

# Copy and make the start script executable
COPY start-services.sh /app/start-services.sh
RUN chmod +x /app/start-services.sh

# Expose the ports for the application and the debugger
EXPOSE 5000 5001 5678

# Copy the .env file into the container
COPY .env .

# Command to run the services
ENTRYPOINT  ["sh", "-c", "/app/start-services.sh"]
