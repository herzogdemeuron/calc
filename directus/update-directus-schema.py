"""
This script updates the Directus schema with the schema from the calc-directus-schema.json file.
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

# Get schema diff
urlSchemaDiff = 'http://localhost:8055/schema/diff'

with open('calc-directus-schema.json', 'r') as f:
    calcSchema = json.load(f)

if not calcSchema:
    print('No schema found!')
    exit()

headers["Authorization"] = 'Bearer {}'.format(access_token)
response = requests.post(urlSchemaDiff, headers=headers, json=calcSchema, params={"force": True})
schemaDiff = response.json()["data"]
print("Schema diff status code: {}".format(response.status_code))


# Apply schema diff
urlSchemaApply = 'http://localhost:8055/schema/apply'
response = requests.post(urlSchemaApply, headers=headers, json=schemaDiff)
print("Schema apply status code: {}".format(response.status_code))