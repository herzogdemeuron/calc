"""
This is just a helper script to get the schema from Directus and save it to a file. It's not needed for applying a schema to Directus.
"""

import requests
import json

# Get access token
urlAuth = 'http://localhost:8055/auth/login'
authBody =  {"email": "admin@example.com", "password": "d1r3ctu5"}

headers = {"Content-Type":"application/json","Accept":"application/json"}
response = requests.post(urlAuth, headers=headers, json=authBody)
print("Auth response status code: {}".format(response.status_code)) 

access_token = response.json()["data"]["access_token"]

# Get schema
urlSchema = 'http://localhost:8055/schema/snapshot'
headers["Authorization"] = 'Bearer {}'.format(access_token)
response = requests.get(urlSchema, headers=headers)
print(response.status_code)

with open('directus-schema.json', 'w') as outfile: 
    json.dump(response.json()["data"], outfile)