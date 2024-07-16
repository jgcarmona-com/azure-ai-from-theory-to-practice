# Detección y Análisis de Rostros con Azure AI Vision

Este laboratorio te guiará a través del proceso de detección y análisis de rostros utilizando Azure AI Vision y el servicio Face API. Exploraremos cómo detectar rostros y analizar sus atributos en imágenes, usando ejemplos prácticos.

## Instrucciones

### 1. Clonar el Repositorio

1. Abre Visual Studio Code.
2. Abre la paleta (SHIFT+CTRL+P) y ejecuta el comando `Git: Clone` para clonar el repositorio [Azure AI from Theory to Practice](https://github.com/jgcarmona-com/azure-ai-from-theory-to-practice) a una carpeta local.
3. Una vez clonado, abre la carpeta en Visual Studio Code.

### 2. Provisión de Recursos en Azure

1. Mientras esté activo el recurso utilizado para esta demo se podrá utilizar sin modificar ninguna de las variables de entorno de los proyectos de ejemplo.
2. Si el recurso no está disponible, tendrás que crear tu propio recurso en tu suscripción de Azure y reemplazar los valores que corresponda en los proyectos de ejemplo, en los ficheros `.env` de Python y en `appsettings.json` de C#.

### 3. Preparar el Entorno para el SDK de Azure AI Vision

1. En Visual Studio Code, en el panel Explorer, navega a la carpeta `07-face/computer-vision`.
2. Abre una terminal integrada en esta carpeta y ejecuta el siguiente comando para instalar el paquete del SDK de Azure AI Vision:

    **C#**:
    ```sh
    dotnet add package Azure.AI.Vision.ImageAnalysis -v 0.15.1-beta.1
    ```

    **Python**:
    ```sh
    pip install azure-ai-vision==0.15.1b1
    ```

3. Abre el archivo de configuración `appsettings.json` (C#) o `.env` (Python) y actualiza los valores de `endpoint` y `key` con los de tu recurso de Azure AI Services. Guarda los cambios.

### 4. Detectar Rostros en una Imagen

1. Ejecuta el programa desde la terminal integrada para detectar rostros en una imagen.

    **C#**:
    ```sh
    dotnet run
    ```

    **Python**:
    ```sh
    python detect-people.py
    ```

### 5. Usar el Face API para Análisis Más Completo

1. En Visual Studio Code, en el panel Explorer, navega a la carpeta `07-face/face-api`.
2. Abre una terminal integrada en esta carpeta y ejecuta el siguiente comando para instalar el paquete del SDK del Face API:

    **C#**:
    ```sh
    dotnet add package Microsoft.Azure.CognitiveServices.Vision.Face --version 2.8.0-preview.3
    ```

    **Python**:
    ```sh
    pip install azure-cognitiveservices-vision-face==0.6.0
    ```

3. Abre el archivo de configuración `appsettings.json` (C#) o `.env` (Python) y actualiza los valores de `endpoint` y `key` con los de tu recurso de Azure AI Services. Guarda los cambios.

4. Ejecuta el programa desde la terminal integrada para un análisis más completo de los rostros.

    **C#**:
    ```sh
    dotnet run
    ```

    **Python**:
    ```sh
    python analyze-faces.py
    ```

### 6. Ejemplos Didácticos

Los ejemplos didácticos demuestran el flujo completo desde la detección de rostros hasta el análisis utilizando el servicio Face API.

- **Python**: Revisa el archivo `analyze-faces.py` en la carpeta `python/face-api`.
- **C#**: Revisa el archivo `Program.cs` en la carpeta `csharp/face-api`.

### 7. Limpiar Recursos

Si creaste recursos en tu suscripción y no vas a utilizarlos más, deberías eliminarlos para evitar gastar más de la cuenta. Aquí tienes un enlace a la [documentación oficial](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/manage-resources-portal).

---

¡Felicidades! Has completado el laboratorio de detección y análisis de rostros con Azure AI Vision.
