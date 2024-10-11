"""
This is just a helper script to get the schema from Directus and save it to a file. It's not needed for updating Directus with the calc-directus-schema.
Use it for setting up a local Directus instance for testing.
"""

import requests
import json

# change this to your Directus instance:
baseUrl = 'https://hdm-dt.directus.app'  
urlAuth = baseUrl + '/auth/login'
# change this to your Directus credentials:
authBody = {
    "email": "admin@example.com", 
    "password": "d1r3ctu5"
}  

headers = {
    "Content-Type": "application/json",
    "Accept": "application/json"
}

response = requests.post(urlAuth, headers=headers, json=authBody)
print("Auth response status code: {}".format(response.status_code))

access_token = response.json()["data"]["access_token"]
headers["Authorization"] = f'Bearer {access_token}'

# replace with collection names
collections = ['collection1', 'collection2']  

# Loop through each collection and get its schema
all_collections_data = {}
for collection in collections:
    urlCollectionSchema = f'{baseUrl}/collections/{collection}'
    response = requests.get(urlCollectionSchema, headers=headers)
    print(f"Fetching schema for collection '{collection}' - Status: {response.status_code}")
    
    if response.status_code == 200:
        all_collections_data[collection] = response.json()["data"]
    else:
        print(f"Failed to fetch schema for collection: {collection}")

# Save the collection schemas to a file
with open('directus-collections-schema.json', 'w') as outfile:
    json.dump(all_collections_data, outfile)




