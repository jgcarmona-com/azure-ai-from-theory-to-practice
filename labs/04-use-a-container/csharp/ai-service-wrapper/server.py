from fastapi import FastAPI, Request
from pydantic import BaseModel
import requests
import os
import json

app = FastAPI()

# Leer variables de entorno
API_KEY = os.getenv('API_KEY')
BILLING_ENDPOINT = os.getenv('BILLING_ENDPOINT')

class Document(BaseModel):
    id: int
    text: str

class Documents(BaseModel):
    documents: list[Document]

@app.post("/process")
async def process_texts(documents: Documents):
    # Preparar datos para la solicitud al contenedor de Language Detection
    data = documents.dict()

    # Enviar solicitud al contenedor de Language Detection
    response = requests.post(
        'http://localhost:5000/text/analytics/v3.0/languages',
        headers={'Content-Type': 'application/json'},
        data=json.dumps(data)
    )

    # Procesar la respuesta
    if response.status_code == 200:
        results = response.json()
        processed_results = []
        for doc in results['documents']:
            processed_text = doc['text']  # Aquí puedes agregar el procesamiento adicional
            processed_results.append({
                'id': doc['id'],
                'language': doc['detectedLanguage']['name'],
                'processed_text': processed_text
            })
        return {"results": processed_results}
    else:
        return {"error": response.text}, response.status_code

if __name__ == '__main__':
    import uvicorn
    uvicorn.run(app, host='0.0.0.0', port=5000)
