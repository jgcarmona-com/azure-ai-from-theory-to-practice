# azure-ai-from-theory-to-practice

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
   cd azure-ai-from-theory-to-practice
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

...

