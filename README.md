# azure-ai-from-theory-to-practice

# Contenido
- [Descripción](#Descripción)
- [Requisitos](#Requisitos)
- [Configuración](#Configuración)
- [Contenido](#Contenido)
  - [Lab 1: Consumir un servicio de Azure AI](#lab-1-consumir-un-servicio-de-azure-ai)
  - [Lab 2: Seguridad en los servicios de Azure AI](#lab-2-seguridad-en-los-servicios-de-azure-ai)
  - [Lab 3: Monitoreo de servicios de Azure AI](#lab-3-monitoreo-de-servicios-de-azure-ai)
  - [Lab 4: Uso de contenedores](#lab-4-uso-de-contenedores)
  - [Lab 5: Análisis de imágenes](#lab-5-análisis-de-imágenes)
  - [Lab 6: Clasificación de Imágenes con Azure AI Vision](#lab-6-clasificación-de-imágenes-con-azure-ai-vision)
  - [Lab 7: Detección de Personas](#lab-7-detección-de-personas)
  - [Lab 8: Reconocimiento Óptico de Caracteres (OCR)](#lab-8-reconocimiento-óptico-de-caracteres-ocr)
  - [Lab 9: Indexación de Videos](#lab-9-indexación-de-videos)
  - [Lab 10: Análisis de Texto](#lab-10-análisis-de-texto)
  - [Lab 11: Preguntas y Respuestas (QnA Bot)](#lab-11-creación-de-un-bot-de-preguntas-y-respuestas-qna-bot)
  - [Lab 12: Language Understanding](#lab-12-creación-del-aventubot-para-actividades-de-aventura)
  - [Lab 13: Clasificación de Texto Personalizada](#lab-13-clasificación-de-texto-personalizada)



## Descripción

Este repositorio contiene el código y los recursos para el curso "Inteligencia Artificial con Azure: De la Teoría a la Práctica". El objetivo del curso es enseñar a los desarrolladores cómo crear, consumir y gestionar servicios de Inteligencia Artificial en Azure, utilizando tanto C# como Python.

En la carpeta labs encontrarás una carpeta por cada lenguaje utilizado, por ejemplo, en el primer lab tienes cshap y python, los lenguajes con los que, en general, se trabajará en el curso.

## Requisitos

Para poder realizar los laboratorios necesitarás una suscripción de Azure. Si no tienes una, puedes crear una cuenta gratuita en [https://azure.com/free](https://azure.com/free).

# Configuración
Sigue estos pasos para configurar tu entorno de desarrollo:

1. Clona el repositorio:
   ```bash
   git clone https://github.com/jgcarmona-com/azure-ai-from-theory-to-practice.git
   ```
2. Navega hasta el laboratorio que quieras probar y abre la carpeta python o csharp con VSCode o tu IDE favorito.
3. Para los ejemplos de Python es recomendable crear un entorno virtual
   ```bash
   cd azure-ai-from-theory-to-practice/labs/xyz/python
   python -m venv .venv
   source .venv/bin/activate  # En Windows: .venv\Scripts\activate
   ```
   También tendrás que instalar las correspondientes dependencias:
   ```bash
   pip install -r requirements.txt
   ```
## Contenido

### Lab 1: Consumir un servicio de Azure AI

En este laboratorio aprenderás a consumir un servicio de Azure AI desde una aplicación de consola en C# y Python, tanto utilizando REST como con el SDK de Azure.

- **csharp**
  - `rest-client`: Cliente REST para consumir servicios de Azure AI.
  - `sdk-client`: Cliente basado en SDK para consumir servicios de Azure AI.
  - `csharp.sln`: Solución de Visual Studio para los proyectos de C#.

- **python**
  - `rest-client`: Cliente REST para consumir servicios de Azure AI en Python.
  - `sdk-client`: Cliente basado en SDK para consumir servicios de Azure AI en Python.

### Lab 2: Seguridad en los servicios de Azure AI

En este laboratorio aprenderás a asegurar los servicios de Azure AI utilizando Azure Key Vault y autenticación basada en tokens, tanto en C# como en Python.

- **csharp**
  - `keyvault_client`: Cliente para interactuar con Azure Key Vault.
  - `token-based-client`: Cliente basado en token para consumir servicios de Azure AI de manera segura.
  - `02-ai-services-security.sln`: Solución de Visual Studio para los proyectos de C#.

- **python**
  - `keyvault_client`: Cliente para interactuar con Azure Key Vault en Python.
  - `token-based-client`: Cliente basado en token para consumir servicios de Azure AI de manera segura en Python.
  - `requirements.txt`: Dependencias necesarias para los ejemplos en Python.

### Lab 3: Monitoreo de servicios de Azure AI

En este laboratorio aprenderás a monitorear los servicios de Azure AI utilizando consultas KQL y scripts de prueba REST.

- `kql-queries.txt`: Consultas KQL para monitorear servicios de Azure AI.
- `rest-test.cmd`: Script de prueba REST en Windows.
- `rest-test.sh`: Script de prueba REST en Unix.

### Lab 4: Uso de contenedores

En este laboratorio aprenderás a desplegar servicios de Azure AI utilizando contenedores, con ejemplos en C# y Python.

- **csharp**
  - `ai-service-wrapper`: Wrapper en C# para servicios de Azure AI.

- **python**
  - `ai-service-wrapper`: Wrapper en Python para servicios de Azure AI.

- Archivos adicionales:
  - `.env`: Archivo de configuración de entorno.
  - `Dockerfile`: Archivo Docker para construir la imagen del contenedor.
  - `rest-test-local.sh`: Script de prueba REST local en Unix.
  - `rest-test.cmd`: Script de prueba REST en Windows.
  - `run-container-local.sh`: Script para ejecutar el contenedor localmente en Unix.
  - `run-container.sh`: Script para ejecutar el contenedor en Unix.

### Lab 5: Análisis de imágenes

En este laboratorio aprenderás a analizar imágenes utilizando servicios de Azure AI, con ejemplos en C# y Python.

#### csharp

- **Archivos:**
  - `BackgroundRemover.cs`: Código para eliminar el fondo de imágenes.
  - `ImageAnalyzer.cs`: Código para analizar imágenes utilizando Azure AI.
  - `Program.cs`: Programa principal de ejemplo en C#.
  - `appsettings.json`: Archivo de configuración para la aplicación.
  - `image-analysis.csproj`: Archivo del proyecto C#.

- **Imágenes de ejemplo:**
  - `images/building.jpg`: Imagen de ejemplo de un edificio.
  - `images/person.jpg`: Imagen de ejemplo de una persona.
  - `images/street.jpg`: Imagen de ejemplo de una calle.
  - `images/building_no_bg.jpg`: Imagen de ejemplo de un edificio sin fondo.
  - `images/building_result.jpg`: Imagen de ejemplo del resultado del análisis.

- **Fuente:**
  - `arial.ttf`: Archivo de fuente utilizado para anotar imágenes.

#### python

- **Archivos:**
  - `background_remover.py`: Código en Python para eliminar el fondo de imágenes.
  - `image_analysis.py`: Código en Python para analizar imágenes utilizando Azure AI.
  - `.env`: Archivo de configuración para la aplicación.

- **Imágenes de ejemplo:**
  - `images/building.jpg`: Imagen de ejemplo de un edificio.
  - `images/person.jpg`: Imagen de ejemplo de una persona.
  - `images/street.jpg`: Imagen de ejemplo de una calle.

- **Fuente:**
  - `arial.ttf`: Archivo de fuente utilizado para anotar imágenes.


### Lab 6: Clasificación de Imágenes con Azure AI Vision

En este laboratorio aprenderás a:
- Crear y entrenar un modelo de clasificación de imágenes personalizado utilizando Azure AI Vision y Custom Vision. 
- Consumir la API de predicción de Azure AI Vision para clasificar imágenes.
- Implementar un flujo de trabajo completo de Machine Learning Operations (MLOps) para la creación, entrenamiento y despliegue de un modelo de clasificación de imágenes.

#### ·jemplos disponibles:

#### Python:

- `main.py`: Script para consumir la API de predicción y clasificar imágenes.
- `workflow_example.py`: Ejemplo completo de MLOps.

#### C#:

- `image-classification/Program.cs`: Programa principal para consumir la API de predicción y clasificar imágenes.
- `mlops-example/Program.cs`: Ejemplo completo de MLOps.

### Lab 7: Detección de Personas

En este laboratorio aprenderás a detectar personas en imágenes utilizando servicios de Azure AI, con ejemplos en C# y Python.

#### csharp

- **Archivos:**
  - `detect-people/Program.cs`: Programa principal de ejemplo en C# para detectar personas.
  - `detect-people/appsettings.json`: Archivo de configuración para la aplicación.
  - `detect-people/detect-people.csproj`: Archivo del proyecto C#.

#### python

- **Archivos:**
  - `detect-people/main.py`: Código en Python para detectar personas en imágenes.
  - `detect-people/requirements.txt`: Archivo de requisitos para la aplicación Python.

### Lab 8: Reconocimiento Óptico de Caracteres (OCR)

En este laboratorio aprenderás a realizar OCR en imágenes y documentos utilizando servicios de Azure AI, con ejemplos en C# y Python.

#### csharp

- **Archivos:**
  - `read-text/DocumentProcessingService.cs`: Servicio para procesar documentos.
  - `read-text/ImageProcessingService.cs`: Servicio para procesar imágenes.
  - `read-text/Program.cs`: Programa principal de ejemplo en C#.
  - `read-text/appsettings.json`: Archivo de configuración para la aplicación.
  - `read-text/read-text.csproj`: Archivo del proyecto C#.

#### python

- **Archivos:**
  - `read-text/document_processing_service.py`: Servicio en Python para procesar documentos.
  - `read-text/image_processing_service.py`: Servicio en Python para procesar imágenes.
  - `read-text/main.py`: Programa principal de ejemplo en Python.
  - `read-text/requirements.txt`: Archivo de requisitos para la aplicación Python.

### Lab 9: Indexación de Videos

En este laboratorio aprenderás a analizar videos utilizando el servicio de indexación de videos de Azure.

#### Archivos:

- `analyze-video-editor.html`: Página HTML para analizar videos en un editor.
- `analyze-video-player-insights.html`: Página HTML para reproducir videos con información de análisis.
- `get-videos.ps1`: Script de PowerShell para obtener videos.

### Lab 10: Análisis de Texto

En este laboratorio aprenderás a analizar texto utilizando los servicios de análisis de texto de Azure, con ejemplos en C# y Python.

#### csharp

- **Archivos:**
  - `text-analysis/Program.cs`: Programa principal de ejemplo en C# para analizar texto.
  - `text-analysis/appsettings.json`: Archivo de configuración para la aplicación.
  - `text-analysis/text-analysis.csproj`: Archivo del proyecto C#.

#### python

- **Archivos:**
  - `text-analysis/main.py`: Programa principal de ejemplo en Python para analizar texto.
  - `text-analysis/requirements.txt`: Archivo de requisitos para la aplicación Python.

### Lab 11: Creación de un Bot de Preguntas y Respuestas (QnA Bot)

En este laboratorio aprenderás a crear un bot de preguntas y respuestas utilizando el servicio de Azure AI Language para responder preguntas basadas en un conocimiento predefinido.

#### csharp

- **Archivos:**
  - `qna-bot/Program.cs`: Programa principal de ejemplo en C# para el bot de preguntas y respuestas.
  - `qna-bot/appsettings.json`: Archivo de configuración para la aplicación.
  - `qna-bot/qna-bot.csproj`: Archivo del proyecto C#.

#### python

- **Archivos:**
  - `qna-bot/main.py`: Código en Python para implementar el bot de preguntas y respuestas.
  - `qna-bot/requirements.txt`: Archivo de requisitos para la aplicación Python.

### Lab 12: Creación del AventuBot para Actividades de Aventura

En este laboratorio aprenderás a desarrollar un bot conversacional llamado **AventuBot**. Este bot está diseñado para ayudar a los usuarios a encontrar y reservar actividades de aventura como escalada, senderismo, y más. Utiliza el servicio de Azure AI Language para comprender las intenciones y entidades en el lenguaje natural.

#### csharp

- **Archivos:**
  - `aventubot/Program.cs`: Programa principal de ejemplo en C# para el AventuBot.
  - `aventubot/appsettings.json`: Archivo de configuración para la aplicación.
  - `aventubot/aventubot.csproj`: Archivo del proyecto C#.

#### python

- **Archivos:**
  - `aventubot/main.py`: Código en Python para implementar el AventuBot.
  - `aventubot/requirements.txt`: Archivo de requisitos para la aplicación Python.

### Lab 13: Clasificación de Texto Personalizada

En este laboratorio aprenderás a crear un modelo de clasificación de texto personalizado utilizando el servicio Azure AI Language. El laboratorio incluye la carga de ejemplos de artículos, la etiqueta de datos y la creación de un modelo capaz de clasificar texto en categorías específicas. Aprenderás a entrenar, evaluar y desplegar un modelo para luego consumirlo mediante una aplicación en C# o Python.

#### csharp

- **Archivos:**
  - `classify-text/Program.cs`: Programa principal de ejemplo en C# para clasificar texto.
  - `classify-text/appsettings.json`: Archivo de configuración para la aplicación.
  - `classify-text/classify-text.csproj`: Archivo del proyecto C#.

#### python

- **Archivos:**
  - `classify-text/main.py`: Código en Python para implementar la clasificación de texto.
  - `classify-text/requirements.txt`: Archivo de requisitos para la aplicación Python.

Este contenido está diseñado para demostrar cómo construir un proyecto de clasificación de texto, etiquetar datos, entrenar un modelo y luego desplegarlo para su consumo desde una aplicación real.
