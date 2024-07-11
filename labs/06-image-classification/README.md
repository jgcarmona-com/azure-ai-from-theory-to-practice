# Clasificación de Imágenes con Azure AI Vision

Este laboratorio te guiará a través del proceso de crear y entrenar un modelo de clasificación de imágenes personalizado utilizando Azure AI Vision. Vamos a clasificar imágenes de frutas y predecir su clase usando ejemplos prácticos basados en la [documentación oficial](https://learn.microsoft.com/en-us/azure/ai-services/custom-vision-service/quickstarts/image-classification?tabs=linux%2Cvisual-studio&pivots=programming-language-python).

## Requisitos

- Una suscripción a Azure.
- Familiaridad con Azure y el portal de Azure.
- Visual Studio Code instalado.
- Git instalado.

## Instrucciones

### 1. Clonar el Repositorio

Si no has clonado ya el repositorio, sigue estos pasos:

1. Abre Visual Studio Code.
2. Abre la paleta (SHIFT+CTRL+P) y ejecuta el comando `Git: Clone` para clonar el repositorio [Azure AI from Theory to Practice](https://github.com/jgcarmona-com/azure-ai-from-theory-to-practice) a una carpeta local.
3. Una vez clonado, abre la carpeta en Visual Studio Code.

### 2. Provisión de Recursos en Azure

1. Abre el portal de Azure en [https://portal.azure.com](https://portal.azure.com) y accede con tu cuenta de Microsoft asociada a tu suscripción de Azure.

2. Busca "Azure AI services" en la barra de búsqueda superior, selecciona "Azure AI Services" y crea un recurso de cuenta de servicios múltiples de Azure AI con las siguientes configuraciones:

    - **Subscription**: Tu suscripción de Azure
    - **Resource group**: Selecciona o crea un grupo de recursos
    - **Region**: East US, West Europe, West US 2
    - **Name**: Ingresa un nombre único
    - **Pricing tier**: Standard S0

3. Necesitamos una cuenta de almacenamiento para almacenar las imágenes de entrenamiento:

    - Busca y selecciona "Storage accounts" en el portal de Azure y crea una nueva cuenta de almacenamiento con las siguientes configuraciones:
    - **Subscription**: Tu suscripción de Azure
    - **Resource Group**: Usa el mismo grupo de recursos creado previamente
    - **Storage Account Name**: `customclassifySUFFIX` (reemplaza `SUFFIX` con tus iniciales u otro valor para asegurar que el nombre del recurso sea único a nivel global)
    - **Region**: Usa la misma región que utilizaste para tu recurso de Azure AI Service
    - **Performance**: Standard
    - **Redundancy**: Locally-redundant storage (LRS)

4. Habilita el acceso público en la cuenta de almacenamiento:

    - Navega a "Configuration" en el grupo "Settings" y habilita "Allow Blob anonymous access". Guarda la configuración.
    - Selecciona "Containers" en el panel izquierdo y crea un nuevo contenedor llamado `fruit` con el nivel de acceso anónimo configurado a "Container (anonymous read access for containers y blobs)".

### 3. Actualizar y Ejecutar el Script de PowerShell

1. En Visual Studio Code, expande la carpeta `labs/06-image-classification` y selecciona `replace.ps1`.
2. Reemplaza el marcador de posición `<storageAccount>` en la primera línea del archivo con el nombre de tu cuenta de almacenamiento. Guarda el archivo.
3. Haz clic derecho en la carpeta `06-image-classification` y abre un terminal integrado. Ejecuta el siguiente comando en el terminal:
    ```powershell
    ./replace.ps1
    ```

4. Revisa el archivo `training-images/training_labels.json` para asegurarte de que el nombre de tu cuenta de almacenamiento ha sido reemplazado correctamente.

### 4. Subir Imágenes y COCO File

1. Navega al contenedor `fruit` en tu cuenta de almacenamiento en el portal de Azure.
2. Sube todas las imágenes y el archivo JSON (COCO file) desde la carpeta `training-images` a ese contenedor.

### 5. Crear y Entrenar un Proyecto Personalizado

1. Navega a [https://portal.vision.cognitive.azure.com/](https://portal.vision.cognitive.azure.com/) y accede con la cuenta de Microsoft donde creaste tu recurso de Azure AI.
2. Selecciona el tile "Customize models with images" (puede encontrarse en la pestaña "Image analysis" si no aparece en tu vista predeterminada).
3. Si se te solicita, selecciona el recurso de Azure AI que creaste.
4. En tu proyecto, selecciona "Add new dataset" y configura con los siguientes ajustes:
    - **Dataset name**: `training_images`
    - **Model type**: Image classification
    - **Azure blob storage container**: Selecciona "Select Container"
    - **Subscription**: Tu suscripción de Azure
    - **Storage account**: La cuenta de almacenamiento que creaste
    - **Blob container**: `fruit`
    - Selecciona la casilla "Allow Vision Studio to read and write to your blob storage"

5. Selecciona el dataset `training_images`.

6. En lugar de crear un nuevo proyecto de etiquetado de datos, selecciona "Add COCO file" y en el menú desplegable, selecciona "Import COCO file from a Blob Container". Selecciona `training_labels.json` del contenedor `fruit`.

### 6. Entrenar el Modelo Personalizado

1. Navega a "Custom models" en el panel izquierdo y selecciona "Train a new model" con los siguientes ajustes:
    - **Name of model**: `classifyfruit`
    - **Model type**: Image classification
    - **Choose training dataset**: `training_images`

2. Deja el resto de los ajustes por defecto y selecciona "Train model".

3. La capacitación puede tardar un tiempo; el presupuesto predeterminado es de hasta una hora, pero para este pequeño dataset suele ser mucho más rápido. Selecciona el botón "Refresh" cada pocos minutos hasta que el estado del trabajo sea "Succeeded". Selecciona el modelo para revisar el rendimiento de la capacitación.

### 7. Probar tu Modelo Personalizado

1. En la parte superior de la página de tu modelo personalizado, selecciona "Try it out".
2. Selecciona el modelo `classifyfruit` del menú desplegable y navega a la carpeta `06-image-classification\test-images`.
3. Selecciona cada imagen y revisa los resultados. Selecciona la pestaña JSON en el cuadro de resultados para examinar la respuesta JSON completa.

### 8. Ejemplos Didácticos

He proporcionado ejemplos didácticos en Python y C# que demuestran el flujo completo desde la creación del proyecto hasta la predicción utilizando el modelo entrenado.

- **Python**: Revisa el archivo `WorkflowExample.py` en la carpeta `python/image-classification`.
- **C#**: Revisa el archivo `WorkflowExample.cs` en la carpeta `csharp/image-classification`.

### 9. Limpiar Recursos

Si no vas a utilizar los recursos de Azure creados en este laboratorio para otros módulos de capacitación, puedes eliminarlos para evitar incurrir en cargos adicionales.

1. Abre el portal de Azure en [https://portal.azure.com](https://portal.azure.com) y en la barra de búsqueda superior, busca los recursos que creaste en este laboratorio.
2. En la página del recurso, selecciona "Delete" y sigue las instrucciones para eliminar el recurso. Alternativamente, puedes eliminar todo el grupo de recursos para limpiar todos los recursos al mismo tiempo.

---

¡Felicidades! Has completado el laboratorio de clasificación de imágenes con Azure AI Vision.
