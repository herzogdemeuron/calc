import requests
import json

baseUrl = 'https://xxx.directus.app' # change this to your Directus instance

# Get access token
urlAuth = baseUrl + '/auth/login'
authBody =  {
    "email": "xxx", 
    "password": "xxx"
    } # change this to your Directus credentials

headers = {
    "Content-Type":"application/json",
    "Accept":"application/json"
    }

response = requests.post(urlAuth, headers=headers, json=authBody)
print("Auth response status code: {}".format(response.status_code)) 
# pretty print response
print(json.dumps(response.json(), indent=4, sort_keys=True))

access_token = response.json()["data"]["access_token"]
headers["Authorization"] = 'Bearer {}'.format(access_token)

url = baseUrl + '/items/calc_calculation_results'
data = {
    "query": {
        "limit": -1
    }
}

response = requests.delete(url, headers=headers, json=data)

if response.status_code == 200:
    print('Deletion successful')
else:
    print('Deletion failed. Status code:', response.status_code)