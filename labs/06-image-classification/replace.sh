#!/bin/bash

# Define the storage account name
storageAcct="ai102st"

# Update the JSON file
jq --arg storageAcct "$storageAcct" '
.images |= map(
  .absolute_url |= gsub("<storageAccount>"; $storageAcct)
)' training-images/training_labels.json > training-images/training_labels_temp.json

# Replace the original file with the updated file
mv training-images/training_labels_temp.json training-images/training_labels.json
