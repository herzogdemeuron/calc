"""
This is just a helper script to get the schema from Directus and save it to a file. It's not needed for updating Directus with the calc-directus-schema.
Use it for setting up a local Directus instance for testing.
"""

import requests
import json

baseUrl = 'https://hdm-dt.directus.app' # change this to your Directus instance

# Get access token
urlAuth = baseUrl + '/auth/login'
authBody =  {
    "email": "admin@example.com", 
    "password": "d1r3ctu5"
    } # change this to your Directus credentials

headers = {
    "Content-Type":"application/json",
    "Accept":"application/json"
    }

response = requests.post(urlAuth, headers=headers, json=authBody)
print("Auth response status code: {}".format(response.status_code)) 

access_token = response.json()["data"]["access_token"]

# Get schema
urlSchema = baseUrl + '/schema/snapshot'
headers["Authorization"] = 'Bearer {}'.format(access_token)
response = requests.get(urlSchema, headers=headers)
print(response.status_code)

with open('directus-schema.json', 'w') as outfile: 
    json.dump(response.json()["data"], outfile)


