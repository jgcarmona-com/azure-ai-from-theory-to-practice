#!/bin/bash

# Load the JSON data into a variable
json_data=$(cat training_labels.json)

# Declare associative arrays to hold category names and counters
declare -A category_names
declare -A category_counters

# Initialize the counters
category_names[1]="apple"
category_names[2]="orange"
category_names[3]="banana"
category_counters[1]=1
category_counters[2]=1
category_counters[3]=1

# Create a temporary file for the updated JSON
temp_file=$(mktemp)

# Iterate over the images in the JSON data
for row in $(echo "${json_data}" | jq -r '.images[] | @base64'); do
    _jq() {
        echo "${row}" | base64 --decode | jq -r "${1}"
    }

    image_id=$(_jq '.id')
    file_name=$(_jq '.file_name')

    # Get the category_id for this image_id from annotations
    category_id=$(echo "${json_data}" | jq -r ".annotations[] | select(.image_id == ${image_id}) | .category_id")

    # Get the category name and increment the counter
    category_name=${category_names[${category_id}]}
    counter=${category_counters[${category_id}]}

    # Construct the new file name
    new_file_name="${category_name^^}_${counter}.jpg"

    # Rename the file
    if [ -f "${file_name}" ]; then
        mv "${file_name}" "${new_file_name}"
        echo "Renamed ${file_name} to ${new_file_name}"

        # Update the JSON data with the new file name
        json_data=$(echo "${json_data}" | jq --arg image_id "$image_id" --arg new_file_name "$new_file_name" '
            (.images[] | select(.id == ($image_id | tonumber)) | .file_name) = $new_file_name |
            (.images[] | select(.id == ($image_id | tonumber)) | .coco_url) = ("AmlDatastore://fruit/" + $new_file_name) |
            (.images[] | select(.id == ($image_id | tonumber)) | .absolute_url) = ("https://<storageAccount>.blob.core.windows.net/fruit/" + $new_file_name)
        ')

        # Increment the counter for this category
        category_counters[${category_id}]=$((counter + 1))
    else
        echo "File ${file_name} not found"
    fi
done

# Save the updated JSON data back to the file
echo "${json_data}" > training_labels.json

# Clean up temporary file
rm "$temp_file"

echo "Updated training_labels.json with new file names."
