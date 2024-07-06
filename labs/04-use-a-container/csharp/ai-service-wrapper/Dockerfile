FROM mcr.microsoft.com/azure-cognitive-services/textanalytics/language:latest

# Instalar Python y dependencias adicionales para procesamiento de texto
RUN apt-get update && \
    apt-get install -y python3 python3-pip dos2unix && \
    pip3 install -r requirements.txt

# Copiar el archivo .env al contenedor
COPY .env .env

# Copiar el script del servidor y de procesamiento al contenedor
COPY server.py /server.py
COPY requirements.txt /requirements.txt

# Convertir el archivo .env al formato Unix
RUN dos2unix .env

# Exponer el puerto
EXPOSE 5000

# Comando para iniciar el contenedor
CMD ["uvicorn", "server:app", "--host", "0.0.0.0", "--port", "5000"]
