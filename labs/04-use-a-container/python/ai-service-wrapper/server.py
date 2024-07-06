from fastapi import FastAPI, Request
from pydantic import BaseModel
import requests
import os
import json

app = FastAPI()

# Read environment variables
API_KEY = os.getenv('API_KEY')
BILLING_ENDPOINT = os.getenv('BILLING_ENDPOINT')

class Document(BaseModel):
    id: int
    text: str

class Documents(BaseModel):
    documents: list[Document]

@app.post("/process")
async def process_texts(documents: Documents):
    # Prepare data for the request to the Language Detection container
    data = documents.dict()

    # Send request to the Language Detection container
    response = requests.post(
        'http://localhost:5000/text/analytics/v3.0/languages',
        headers={'Content-Type': 'application/json'},
        data=json.dumps(data)
    )

    # print response
    print(response.text)
    
    # Process the response
    if response.status_code == 200:
        results = response.json()
        return {"results": results}
    else:
        return {"error": response.text}, response.status_code

if __name__ == '__main__':
    import uvicorn
    uvicorn.run(app, host='0.0.0.0', port=5000)
