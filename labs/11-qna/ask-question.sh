
prediction_url="YOUR_ENDPOINT_URL"
key="YOUR_KEY"

curl -X POST $prediction_url -H "Ocp-Apim-Subscription-Key: $key" -H "Content-Type: application/json" -d "{'question': 'Tell me about OpenAI?' }"

