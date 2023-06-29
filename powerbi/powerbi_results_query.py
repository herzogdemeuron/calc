import requests
import pandas as pd

# set token here:
token = "<your-directus-token>"

# set url here:
url = "https://<your-project-name>.directus.app/items"

headers = {'Accept': 'application/json', 'Authorization': 'Bearer {}'.format(token), 'Content-Type': 'application/json'}
res = requests.get("{}/calc_calculation_results".format(url), headers=headers, allow_redirects=False, params={"limit": 300, "sort": "-date_created"})
data = res.json()["data"]
directus = pd.json_normalize(data, max_level=0)
