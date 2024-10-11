"""
This script adds the schema from the calc-directus-schema.json file to the schema of a Directus instance.
"""

import requests
import json

# Set these variables
baseUrl = 'http://localhost:8055'
email = 'admin@example.com'
password = 'd1r3ctu5'

# prepare headers
headers = {"Content-Type":"application/json","Accept":"application/json"}

# Get access token
print("Getting access token...")
urlAuth = baseUrl + '/auth/login'
authBody =  {"email": email, "password": password}
response = requests.post(urlAuth, headers=headers, json=authBody)
print("Auth response status code: {}".format(response.status_code)) 
access_token = response.json()["data"]["access_token"]

# Set auth header
headers["Authorization"] = 'Bearer {}'.format(access_token)

# get existing schema
print("Getting existing schema...")
urlExistingSchema = baseUrl + '/schema/snapshot'
response = requests.get(urlExistingSchema, headers=headers)
print("Get existing status code: {}".format(response.status_code))
try:
    print(json.dumps(response.json(), indent=4, sort_keys=True))
except:
    input("Unable to get schema. Press enter to exit.")
    exit()

existingSchema = response.json()["data"]

# Get schema diff
urlSchemaDiff = baseUrl + '/schema/diff'

with open('./quick_start/directus/directus-schema.json', 'r') as f:
    calcSchema = json.load(f)

if not calcSchema:
    input('No calc schema found! Press enter to exit.')
    exit()

# merge existing schema with calc schema
existingSchema["collections"] += calcSchema["collections"]
existingSchema["fields"] += calcSchema["fields"]
existingSchema["relations"] += calcSchema["relations"]

print("Getting schema diff...")
response = requests.post(urlSchemaDiff, headers=headers, json=existingSchema, params={"force": True})
print("Schema diff status code: {}".format(response.status_code))
try:
    print(response.json())
except:
    input("Looks like your directus is already up to date. Press enter to exit.")
    exit()

schemaDiff = response.json()["data"]

# write diff to file
with open('schema-diff.json', 'w') as f:
    json.dump(schemaDiff, f)

input("schema-diff.json created, go check it out. If you find anything like 'kind: 'D' you should probably stop here, since it means something will be deleted. Press enter to to apply changes to database, ctrl+c to cancel.")
input("Are you sure you want to apply the schema diff? Press enter to continue, ctrl+c to abort.")

# Apply schema diff
print("Applying schema diff...")
urlSchemaApply = baseUrl + '/schema/apply'
response = requests.post(urlSchemaApply, headers=headers, json=schemaDiff)
print("Schema apply status code: {}".format(response.status_code))
try:
    print(response.json())
except:
    print("looks like everything went well, go check your data model")