import requests
import json
import os
from datetime import datetime


# Directus API configuration
BASE_URL = "https://hdm-dt.directus.app/"
API_TOKEN = os.environ.get("REVITRON_DIRECTUS_TOKEN")
if not API_TOKEN:
    raise ValueError("REVITRON_DIRECTUS_TOKEN environment variable is not set")

headers = {
    "Authorization": f"Bearer {API_TOKEN}",
    "Content-Type": "application/json"
}

def get_collection_schema(collection_name):
    """Fetch the schema snapshot of a specific collection."""   
    url = f"{BASE_URL}/schema/snapshot"
    response = requests.get(url, headers=headers)
    response.raise_for_status()
    return response.json()['data']

def modify_schema(schema):
    """Modify the schema as needed."""
    # Example: Rename a field
    if 'fields' in schema:
        for field in schema['fields']:
            if field['field'] == 'old_field_name':
                field['field'] = 'new_field_name'
    
    # Add more modifications as needed
    return schema

def create_new_collection(new_collection_name, modified_schema):
    """Create a new collection with the modified schema."""
    url = f"{BASE_URL}/collections"
    modified_schema['collection'] = new_collection_name
    response = requests.post(url, headers=headers, json=modified_schema)
    response.raise_for_status()
    return response.json()


def main():
    # Get the original schema
    collection_name = "calc_buildups"
    original_schema = get_collection_schema(collection_name)
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    filename = f"{collection_name}_{timestamp}.json"

    with open(filename, 'w') as f:
        json.dump(original_schema, f, indent=2)

    print(f"Schema saved to {filename}")
    # Modify the schema
    #modified_schema = modify_schema(original_schema)

    # Create a new collection with the modified schema
    #result = create_new_collection(new_collection, modified_schema)
    #print(f"New collection created: {result}")

if __name__ == "__main__":
    main()