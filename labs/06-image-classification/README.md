# Clasificación de Imágenes con Azure AI Vision

Este laboratorio te guiará a través del proceso de crear y entrenar un modelo de clasificación de imágenes personalizado utilizando Azure AI Vision y [https://www.customvision.ai/](https://www.customvision.ai/). Vamos a clasificar imágenes de frutas y predecir su clase usando ejemplos prácticos basados en la [documentación oficial](https://learn.microsoft.com/en-us/azure/ai-services/custom-vision-service/quickstarts/image-classification).

## Requisitos

- Una suscripción a Azure.
- Visual Studio Code.
- Git.

## Instrucciones

### 1. Clonar el Repositorio

Si no has clonado ya el repositorio, sigue estos pasos:

1. Abre Visual Studio Code.
2. Abre la paleta (SHIFT+CTRL+P) y ejecuta el comando `Git: Clone` para clonar el repositorio [Azure AI from Theory to Practice](https://github.com/jgcarmona-com/azure-ai-from-theory-to-practice) a una carpeta local.
3. Una vez clonado, abre la carpeta en Visual Studio Code.

### 2. Provisión de Recursos en Azure

1. Mientras esté activo el recurso utilizado para esta demo se podrá utilizar sin modificar ninguna de las variables de entorno de los proyectos de ejemplo.

2. Si el recurso no está disponible, tendrás que crear tu propio recurso en tu suscripción de Azure y reemplazar los valores que corresponda en los proyectos de ejemplo, en los ficheros .env, de python y en appssettings.json, de C#.

### 3. Crear y Entrenar un Proyecto Personalizado

1. Navega a [https://www.customvision.ai/](https://www.customvision.ai/) y accede con la cuenta de Microsoft donde creaste tu recurso de Azure AI.
2. Crea un nuevo proyecto de clasificación de imágenes. Configura el proyecto como de tipo **Multiclass**.
3. Añade las imágenes de entrenamiento desde la carpeta `training-images` y etiqueta cada imagen según su nombre de archivo. (Por ejemplo, si la imagen se llama `apple_01.jpg`, la etiqueta será `apple`, te recomiendo que subas todas las imagenes de cada tipo de una vez y añadas su correspondiente tag en la subida, esto te ahorrará tiempo).

### 4. Entrenar el Modelo Personalizado

1. Desde CustomVision.ai, selecciona el proyecto que creaste y entrena el modelo utilizando las imágenes etiquetadas.
2. Una vez completado el entrenamiento, publica el modelo.

### 5. Probar tu Modelo Personalizado

PAra probar tu modelo personalizado, puedes utilizar la API de predicción de Azure AI Vision. La URL de la API y la clave de predicción están disponibles en la sección de Performance de tu proyecto, haz click en la iteración que quieres probar y luego en "Prediction URL" para ver esos valores.

Los ejemplos están disponibles en Python y C# y puedes lanzarlos directamente desde Visual Studio Code si lo has abierto en la carpeta raíz del repositorio. Hay cuatro ejemplos disponibles:

Clasificación de Imágenes:

Consumo de la API de predicción expuesta por Azure AI Vision para clasificar imágenes.

- **Python**: Revisa el archivo `main.py` en la carpeta `python/image-classification`.
- **C#**: Revisa el archivo `Program.cs` en la carpeta `csharp/image-classification`.

ML Ops:

Ejemplo de flujo de trabajo completo para 
1. La creación de un proyecto.
2. La adición de etiquetas basadas en los nombres de los archivos en la carpeta de imágenes de entrenamiento.
3. La subida de imágenes con sus respectivas etiquetas.
4. El entrenamiento del modelo.
5. La prueba del modelo con las imágenes en la carpeta de `test-images`.

- **Python**: Revisa el archivo `workflow_example.py` en la carpeta `python/image-classification`.
- **C#**: Revisa el archivo `Program.cs` en la carpeta `csharp/mlops-example`.

### 6. Ejemplos Didácticos

Los ejemplos didácticos demuestran el flujo completo desde la creación del proyecto hasta la predicción utilizando el modelo entrenado.

- **Python**: Revisa el archivo `workflow_example.py` en la carpeta `python/image-classification`.
- **C#**: Revisa el archivo `Program.cs` en la carpeta `csharp/mlops-example`.

### 7. Limpiar Recursos

Si creaste recursos en tu suscripción y no vas a utilizarlos más, deberías eliminarlos para evitar gastar más de la cuenta. Seguro que no te hace falta que te explique cómo hacerlo, pero si tienes alguna duda, aquí tienes un enlace a la [documentación oficial](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/manage-resources-portal).

---

¡Felicidades! Has completado el laboratorio de clasificación de imágenes con Azure AI Vision.
